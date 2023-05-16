using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SolarEdge.Monitor.Inverter
{
    public interface IFactory
    {
        IInverter Create();
    }

    public class Factory : IFactory
    {
        private readonly IOptions<Config> _config;
        private readonly ILogger<Instance> _logger;

        public Factory(IOptions<Config> config, ILogger<Instance> logger)
        {
            _config = config;
            _logger = logger;
        }

        public IInverter Create()
        {
            return new Instance(_config.Value.Address, _config.Value.Port, _logger);
        }
    }
}
