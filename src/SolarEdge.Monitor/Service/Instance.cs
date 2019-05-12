using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Service
{
    public class Instance : IHostedService
    {
        private readonly ILogger<Instance> _logger;
        private readonly IOptions<Config> _config;
        private readonly State.IMachine _stateMachine;
        private readonly Message.IFactory _messageFactory;
        private readonly Message.IPublisher _messagePublisher;
        private IDisposable _subscription;

        public Instance(ILogger<Instance> logger, IOptions<Config> config, State.IMachine stateMachine, Message.IFactory messageFactory, Message.IPublisher messagePublisher)
        {
            _logger = logger;
            _config = config;
            _stateMachine = stateMachine;
            _messageFactory = messageFactory;
            _messagePublisher = messagePublisher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription = _stateMachine.Start();

            return Task.CompletedTask;
            //await _inverter.ConnectAsync("192.168.2.23", 502);
            //await _messagePublisher.ConnectAsync();

            //_subscription = Observable
            //    .Interval(TimeSpan.FromSeconds(5), _scheduler)
            //    .SelectMany(_ => _inverter.GetCurrentGenerationAsync())
            //    .Retry()
            //    .Select(_messageFactory.Create)
            //    .Do(message => _logger.Log(LogLevel.Information, message))
            //    .Subscribe(message => _messagePublisher.PublishAsync(message));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }

            return Task.CompletedTask;
            //await _messagePublisher.DisconnectAsync();
            //await _inverter.DisconnectAsync();
        }
    }
}
