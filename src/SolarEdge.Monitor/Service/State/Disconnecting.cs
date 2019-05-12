using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Service.State
{
    public class Disconnecting : IState
    {
        private readonly ILogger<IMachine> _logger;
        private readonly IInverter _inverter;
        private readonly Transition.IFactory _transitionFactory;

        public Disconnecting(ILogger<IMachine> logger, IInverter inverter, Transition.IFactory transitionFactory)
        {
            _logger = logger;
            _inverter = inverter;
            _transitionFactory = transitionFactory;
        }

        private async Task<ITransition> Disconnect()
        {
            await _inverter.DisconnectAsync();

            return _transitionFactory.ToDisconnected();
        }

        public IObservable<ITransition> Enter()
        {
            _logger.Log(LogLevel.Information, "Entered 'Disconnecting' state");

            return Observable.StartAsync(Disconnect);
        }
    }
}
