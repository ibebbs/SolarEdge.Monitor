using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SolarEdge.Monitor.Service.State
{
    public interface IFactory
    {
        IState Disconnected();
        IState Connecting();
        IState Connected(Transition.ToConnected transition);
        IState Disconnecting(Transition.ToDisconnecting transition);
    }

    public class Factory : IFactory
    {
        private readonly ILogger<IMachine> _logger;
        private readonly IOptions<Config> _config;
        private readonly Inverter.IFactory _inverterFactory;
        private readonly Transition.IFactory _transitionFactory;
        private readonly Message.IFacade _messageFacade;

        public Factory(ILogger<IMachine> logger, IOptions<Config> config, Inverter.IFactory inverterFactory, Transition.IFactory transitionFactory, Message.IFacade messageFacade)
        {
            _logger = logger;
            _config = config;
            _inverterFactory = inverterFactory;
            _transitionFactory = transitionFactory;
            _messageFacade = messageFacade;
        }

        public IState Connected(Transition.ToConnected transition)
        {
            return new Connected(_logger, _config, transition.Inverter, _messageFacade, _transitionFactory);
        }

        public IState Connecting()
        {
            return new Connecting(_logger, _config, _inverterFactory, _transitionFactory);
        }

        public IState Disconnecting(Transition.ToDisconnecting transition)
        {
            return new Disconnecting(_logger, transition.Inverter, _transitionFactory);
        }

        public IState Disconnected()
        {
            return new Disconnected(_logger, _transitionFactory);
        }
    }
}
