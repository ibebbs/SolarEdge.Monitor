using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq;
using SolarEdge.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Service.State
{
    public interface IDataExtract
    {
        void Publish(Message.IFacade messagingFacade);
    }

    public class DataExtract<T> : IDataExtract
        where T : Inverter.Model.Instance
    {
        private readonly T _value;

        public DataExtract(T value)
        {
            _value = value;
        }

        public void Publish(Message.IFacade messagingFacade)
        {
            messagingFacade.Publish(_value);
        }
    }

    public interface IDataExtractor
    {
        Task<IDataExtract> Read(IInverter inverter);
    }

    public class DataExtractor<T> : IDataExtractor
        where T : Inverter.Model.Instance
    {
        private readonly Func<IInverter, Task<T>> _read;

        public DataExtractor(Func<IInverter, Task<T>> read)
        {
            _read = read;
        }

        public async Task<IDataExtract> Read(IInverter inverter)
        {
            T value = await _read(inverter);

            var dataExtract = new DataExtract<T>(value);

            return dataExtract;
        }
    }

    public class Connected : IState
    {
        private static readonly IReadOnlyDictionary<string, Func<IDataExtractor>> ModelToExtractorMappings = new Dictionary<string, Func<IDataExtractor>>
        {
            { nameof(Inverter.Model.Inverter), () => new DataExtractor<Inverter.Model.Inverter>(inv => inv.Get<Inverter.Model.Inverter>()) },
            { nameof(Inverter.Model.Meter1Info), () => new DataExtractor<Inverter.Model.Meter1Info>(inv => inv.Get<Inverter.Model.Meter1Info>()) },
            { nameof(Inverter.Model.Meter1Readings), () => new DataExtractor<Inverter.Model.Meter1Readings>(inv => inv.Get<Inverter.Model.Meter1Readings>()) }
        };

        private readonly ILogger<IMachine> _logger;
        private readonly IOptions<Config> _config;
        private readonly IInverter _inverter;
        private readonly Message.IFacade _messageFacade;
        private readonly Transition.IFactory _transitionFactory;
        private readonly IScheduler _scheduler;
        private readonly IEnumerable<IDataExtractor> _extractors;

        public Connected(ILogger<IMachine> logger, IOptions<Config> config, IInverter inverter, Message.IFacade messageFacade, Transition.IFactory transitionFactory)
        {
            _logger = logger;
            _config = config;
            _inverter = inverter;
            _messageFacade = messageFacade;
            _transitionFactory = transitionFactory;
            _scheduler = TaskPoolScheduler.Default;

            _extractors = config.Value.ModelsToRead
                .Split(',')
                .Join(ModelToExtractorMappings, name => name.ToLower(), mapping => mapping.Key.ToLower(), (name, mapping) => mapping.Value())
                .ToArray();
        }

        private IObservable<ITransition> DisconnectionEventTransition()
        {
            return Observable
                .FromEvent<ConnectionEvent, object>(
                    handler => new ConnectionEvent(sender => handler(sender)),
                    handler => _inverter.Disconnected += handler,
                    handler => _inverter.Disconnected -= handler)
                .Do(_ => _logger.LogError("Inverter disconnected"))
                .Select(_ => _transitionFactory.ToDisconnecting(_inverter));
        }

        private Task<IEnumerable<IDataExtract>> ReadExtractors()
        {
            _logger.LogInformation("Polling inverter");

            return _extractors
                .SelectAsync(extractor => extractor.Read(_inverter))
                .Memoize();
        }

        private void PublishExtracts(IEnumerable<IDataExtract> extracts)
        {
            extracts.ForEach(extract => extract.Publish(_messageFacade));
        }

        private IObservable<ITransition> PublishUntilErrorTransition()
        {
            return Observable.Create<ITransition>(
                observer =>
                {
                    var inverter = Observable
                        .Interval(TimeSpan.FromSeconds(_config.Value.PollingIntervalSeconds), _scheduler)
                        .SelectMany(_ => Fallible.AsyncOperation(ReadExtractors))
                        .Publish();

                    var successSubscription = inverter
                        .Where(fallible => fallible.IsSuccessful)
                        .Subscribe(fallible => PublishExtracts(fallible.Value));

                    var failedSubscription = inverter
                        .Where(fallible => fallible.IsError)
                        .Do(fallible => _logger.Log(LogLevel.Error, fallible.Exception, "Encountered exception while querying inverter"))
                        .Select(fallible => _transitionFactory.ToDisconnecting(_inverter))
                        .Subscribe(observer);

                    return new CompositeDisposable(
                        failedSubscription,
                        successSubscription,
                        inverter.Connect()
                    );
                });
        }

        public IObservable<ITransition> Enter()
        {
            _logger.Log(LogLevel.Information, "Entered 'Connected' state");

            return Observable.Merge(
                DisconnectionEventTransition(),
                PublishUntilErrorTransition()
            );
        }
    }
}
