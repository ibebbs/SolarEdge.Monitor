using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;

namespace SolarEdge.Monitor.Service.State
{
    public class Disconnected : IState
    {
        private readonly ILogger<IMachine> _logger;
        private readonly Transition.IFactory _transitionFactory;

        public Disconnected(ILogger<IMachine> logger, Transition.IFactory transitionFactory)
        {
            _logger = logger;
            _transitionFactory = transitionFactory;
        }

        public IObservable<ITransition> Enter()
        {
            _logger.Log(LogLevel.Information, "Entered 'Disconnected' state");

            return Observable.Return(_transitionFactory.ToConnecting()).Delay(TimeSpan.FromSeconds(5));
        }
    }
}
