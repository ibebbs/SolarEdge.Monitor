using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SolarEdge.Monitor.Service.State
{
    public interface IMachine
    {
        IDisposable Start();
    }

    public class Machine : IMachine
    {
        private readonly ILogger<IMachine> _logger;
        private readonly IFactory _factory;
        private readonly Subject<IState> _state;

        public Machine(ILogger<IMachine> logger, IFactory factory)
        {
            _logger = logger;
            _factory = factory;

            _state = new Subject<IState>();
        }

        public IDisposable Start()
        {
            // First create a stream of transitions by ...
            IObservable<ITransition> transitions = _state
                // ... starting from the initializing state ...
                .StartWith(_factory.Disconnected())
                // ... enter the current state ...
                .Select(state => state.Enter())
                // ... subscribing to the transition observable ...
                .Switch()
                // ... and ensure only a single shared subscription is made to the transitions observable ...
                .Publish()
                // ... held until there are no more subscribers
                .RefCount();

            // Then, for each transition type, select the new state...
            IObservable<IState> states = Observable.Merge(
                transitions.OfType<Transition.ToConnecting>().Do(transition => _logger.Log(LogLevel.Information, $"Transitioning '{transition.GetType().Name}'")).Select(transition => _factory.Connecting()),
                transitions.OfType<Transition.ToConnected>().Do(transition => _logger.Log(LogLevel.Information, $"Transitioning '{transition.GetType().Name}'")).Select(transition => _factory.Connected(transition)),
                transitions.OfType<Transition.ToDisconnecting>().Do(transition => _logger.Log(LogLevel.Information, $"Transitioning '{transition.GetType().Name}'")).Select(transition => _factory.Disconnecting(transition)),
                transitions.OfType<Transition.ToDisconnected>().Do(transition => _logger.Log(LogLevel.Information, $"Transitioning '{transition.GetType().Name}'")).Select(transition => _factory.Disconnected())
            );

            // Finally, subscribe to the state observable ...
            return states
                // ... ensuring all transitions are serialized ...
                .ObserveOn(Scheduler.CurrentThread)
                // ... back onto the source state observable
                .Subscribe(_state);
        }
    }
}
