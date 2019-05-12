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

        public Factory(IOptions<Config> config)
        {
            _config = config;
        }

        public IInverter Create()
        {
            return new Instance(_config.Value.Address, _config.Value.Port);
        }
    }
}
