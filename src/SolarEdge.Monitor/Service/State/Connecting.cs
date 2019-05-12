using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Service.State
{
    public class Connecting : IState
    {
        private readonly ILogger<IMachine> _logger;
        private readonly Inverter.IFactory _inverterFactory;
        private readonly Transition.IFactory _transitionFactory;

        public Connecting(ILogger<IMachine> logger, IOptions<Config> config, Inverter.IFactory inverterFactory, Transition.IFactory transitionFactory)
        {
            _logger = logger;
            _inverterFactory = inverterFactory;
            _transitionFactory = transitionFactory;
        }

        private async Task<ITransition> Connect()
        {
            var inverter = _inverterFactory.Create();

            bool connected = await inverter.ConnectAsync(); // "192.168.2.23", 502);

            return connected
                ? _transitionFactory.ToConnected(inverter)
                : _transitionFactory.ToDisconnected();
        }

        public IObservable<ITransition> Enter()
        {
            _logger.Log(LogLevel.Information, "Entered 'Connecting' state");

            return Observable.StartAsync(Connect);
        }
    }
}
