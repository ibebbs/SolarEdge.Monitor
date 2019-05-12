using System;

namespace SolarEdge.Monitor.Inverter.Model
{
    // https://www.solaredge.com/sites/default/files/sunspec-implementation-technical-note.pdf
    public enum Phase : UInt16
    {
        SinglePhase = 101,
        SplitPhase = 102,
        ThreePhase = 103
    }

    public enum OperatingState : UInt16
    {
        Off = 1,            // Off
        Sleeping = 2,       // Sleeping (auto-shutdown) – Night mode
        Starting = 3,       // Grid Monitoring/wake-up
        Producing = 4,      // Inverter is ON and producing power
        Throttled = 5,      // Production (curtailed)
        ShuttingDown = 6,   // Shutting down
        Fault = 7,          // Fault
        Standby = 8         // Maintenance/setup
    }

    public abstract class Instance { }

    [RegisterMapping(40072, RegisterType.Object, Size = 5)]
    public class AcCurrent : Instance
    {
        [RegisterMapping(40072, RegisterType.ScaledUInt16, RegisterUnit.Amps, ScalingFactorAddress = 40076)]
        public float Total { get; set; }
        [RegisterMapping(40073, RegisterType.ScaledUInt16, RegisterUnit.Amps, ScalingFactorAddress = 40076)]
        public float PhaseA { get; set; }
        [RegisterMapping(40074, RegisterType.ScaledUInt16, RegisterUnit.Amps, ScalingFactorAddress = 40076)]
        public float PhaseB { get; set; }
        [RegisterMapping(40075, RegisterType.ScaledUInt16, RegisterUnit.Amps, ScalingFactorAddress = 40076)]
        public float PhaseC { get; set; }
    }

    [RegisterMapping(40077, RegisterType.Object, Size = 7)]
    public class AcVoltages : Instance
    {
        [RegisterMapping(40077, RegisterType.ScaledUInt16, RegisterUnit.Volts, ScalingFactorAddress = 40083)]
        public float PhaseAB { get; set; }
        [RegisterMapping(40078, RegisterType.ScaledUInt16, RegisterUnit.Volts, ScalingFactorAddress = 40083)]
        public float PhaseBC { get; set; }
        [RegisterMapping(40079, RegisterType.ScaledUInt16, RegisterUnit.Volts, ScalingFactorAddress = 40083)]
        public float PhaseCA { get; set; }
        [RegisterMapping(40080, RegisterType.ScaledUInt16, RegisterUnit.Volts, ScalingFactorAddress = 40083)]
        public float PhaseAN { get; set; }
        [RegisterMapping(40081, RegisterType.ScaledUInt16, RegisterUnit.Volts, ScalingFactorAddress = 40083)]
        public float PhaseBN { get; set; }
        [RegisterMapping(40082, RegisterType.ScaledUInt16, RegisterUnit.Volts, ScalingFactorAddress = 40083)]
        public float PhaseCN { get; set; }
    }

    [RegisterMapping(40070, RegisterType.Object, Size = 40)]
    public class Inverter : Instance
    {
        [RegisterMapping(40070, RegisterType.UInt16)]
        public Phase Phase { get; set; }

        [RegisterMapping(40072, RegisterType.Object)]
        public AcCurrent AcCurrent { get; set; }

        [RegisterMapping(40077)]
        public AcVoltages AcComparison { get; set; }

        [RegisterMapping(40084, RegisterType.ScaledInt16, RegisterUnit.Watts, ScalingFactorAddress = 40085)]
        public float AcPowerValue { get; set; }

        [RegisterMapping(40086, RegisterType.ScaledUInt16, RegisterUnit.Hertz, ScalingFactorAddress = 40087)]
        public float AcFrequency { get; set; }

        [RegisterMapping(40088, RegisterType.ScaledInt16, RegisterUnit.VA, ScalingFactorAddress = 40089)]
        public float ApparentPower { get; set; }

        [RegisterMapping(40090, RegisterType.ScaledInt16, RegisterUnit.VAR, ScalingFactorAddress = 40091)]
        public float ReactivePower { get; set; }

        [RegisterMapping(40092, RegisterType.ScaledInt16, RegisterUnit.Percent, ScalingFactorAddress = 40093)]
        public float PowerFactor { get; set; }

        [RegisterMapping(40094, RegisterType.Acc32, RegisterUnit.WattHours, ScalingFactorAddress = 40096)]
        public UInt32 AcLifetimeProduction { get; set; }

        [RegisterMapping(40097, RegisterType.ScaledUInt16, RegisterUnit.Amps, ScalingFactorAddress = 40098)]
        public float DcCurrentValue { get; set; }

        [RegisterMapping(40099, RegisterType.ScaledUInt16, RegisterUnit.Volts, ScalingFactorAddress = 40100)]
        public float DcVoltageValue { get; set; }

        [RegisterMapping(40101, RegisterType.ScaledInt16, RegisterUnit.Watts, ScalingFactorAddress = 40102)]
        public float DcPowerValue { get; set; }

        [RegisterMapping(40104, RegisterType.ScaledInt16, RegisterUnit.Celcius, ScalingFactorAddress = 40107)]
        public float HeatSinkTemperature { get; set; }

        [RegisterMapping(40108, RegisterType.UInt16)]
        public OperatingState OperatingState { get; set; }
    }
}
