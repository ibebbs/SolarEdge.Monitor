using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolarEdge.Monitor.Service
{
    public class Config
    {
        public uint PollingIntervalSeconds { get; set; } = 10;

        [Required]
        public string ModelsToRead { get; set; } = "inverter";
    }
}
