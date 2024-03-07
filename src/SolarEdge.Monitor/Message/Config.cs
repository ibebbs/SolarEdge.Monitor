using System.ComponentModel.DataAnnotations;

namespace SolarEdge.Monitor.Message
{
    public class Config
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The IP address for an MQTT broker is required. You must specify 'MQTT:Address'")]
        public string Address { get; set; }

        [Range(1024, 65536, ErrorMessage = "The specified port for the MQTT broker is out of range. You must specify a valid 'MQTT:Port' in the range of 1024-65536")]
        public int Port { get; set; } = 1883;

        [StringLength(23, ErrorMessage = "The specified client id is possibly too long for some MQTT brokers. You must specify a 'MQTT:ClientId' of less than 24 characters")]
        public string ClientId { get; set; } = "InverterMonitor";

        public string TopicPattern { get; set; } = "home/solar/{type}";

        public ushort KeepAliveSecs { get; set; } = 5;
    }
}
