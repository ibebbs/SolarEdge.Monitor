using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Message
{
    public interface IPublisher
    {
        Task ConnectAsync();

        Task PublishAsync<T>(string message);

        Task DisconnectAsync();

        bool Connected { get; }
    }

    public class Publisher : IPublisher
    {
        private readonly ILogger<IPublisher> _logger;
        private readonly IOptions<Config> _config;
        private IMqttClient _client;
        private SessionState _session;

        public Publisher(ILogger<IPublisher> logger, IOptions<Config> config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task ConnectAsync()
        {
            _logger.LogInformation($"Connecting to MQTT broker at '{_config.Value.Address}:{_config.Value.Port}'");
            _client = await MqttClient.CreateAsync(_config.Value.Address, new MqttConfiguration { Port = _config.Value.Port });
            _session = await _client.ConnectAsync(new MqttClientCredentials(clientId: _config.Value.ClientId));
            _logger.LogInformation($"Successfully connected to MQTT broker at '{_config.Value.Address}:{_config.Value.Port}'");
        }

        private string GetTopic(IReadOnlyDictionary<string, string> properties)
        {
            return properties
                .Aggregate(_config.Value.TopicPattern, (topic, kvp) => topic.Replace($"{{{kvp.Key}}}", kvp.Value))
                .ToLower();
        }

        public async Task PublishAsync<T>(string message)
        {
            var topic = GetTopic(new Dictionary<string, string> { { "type", typeof(T).Name } });

            _logger.Log(LogLevel.Information, $"Publishing to '{topic}':\r\n{message}");

            var mqttMessage = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(message));

            try
            {
                await _client.PublishAsync(mqttMessage, MqttQualityOfService.AtMostOnce);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Encountered exception while trying to publish message");

                _client.Dispose();
                _client = null;
            }
        }

        public async Task DisconnectAsync()
        {
            await _client.DisconnectAsync();
        }

        public bool Connected => _client != null;
    }
}
