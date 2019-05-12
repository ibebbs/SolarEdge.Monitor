using System.ComponentModel.DataAnnotations;

namespace SolarEdge.Monitor.Inverter
{
    public class Config
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The IP address for the inverter is required. You must specify 'Inverter:Address'")]
        public string Address { get; set; }

        [Range(502, 65536, ErrorMessage = "The specified port for the inverter is out of range. You must specify a valid 'Inverter:Port' in the range of 502-65536")]
        public int Port { get; set; } = 502;
    }
}
