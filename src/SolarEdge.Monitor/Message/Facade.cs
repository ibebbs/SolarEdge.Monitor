using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Message
{
    public interface IFacade
    {
        Task Publish<T>(T value) where T : Inverter.Model.Instance;
    }

    public class Facade : IFacade
    {
        private readonly IOptions<Config> _config;
        private readonly ILogger<IFacade> _logger;
        private readonly IFactory _factory;
        private readonly IPublisher _publisher;

        public Facade(IOptions<Config> config, ILogger<IFacade> logger, IFactory factory, IPublisher publisher)
        {
            _config = config;
            _logger = logger;
            _factory = factory;
            _publisher = publisher;
        }

        public async Task Publish<T>(T value)
            where T : Inverter.Model.Instance
        {
            var message = _factory.Create<T>(value);

            try
            {
                if (!_publisher.Connected)
                {
                    await _publisher.ConnectAsync();
                }

                await _publisher.PublishAsync<T>(message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Encountered exception while publishing message");
            }
        }
    }
}
