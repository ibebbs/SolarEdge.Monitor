using System;

namespace SolarEdge.Monitor.Inverter.Model
{
    // https://r1spn12mh523ib7ly1ip4xkn-wpengine.netdna-ssl.com/wp-content/uploads/2016/10/WNC-Modbus-Manual-V18c.pdf
    public enum ModbusType : UInt16
    {
        //Single Phase (AN or AB) Meter(201)
        SinglePhase = 201,
        //Split Single Phase(ABN) Meter(202)
        SplitPhase = 202,
        //Wye-Connect Three Phase(ABCN) Meter(203)
        WyeConnectThreePhase = 203,
        //Delta-Connect Three Phase(ABC) Meter(204)
        DeltaConnectThreePhase = 204
    }

    [Flags]
    public enum MeterEvents : UInt32
    {
        PowerFailure = 0x00000004, // Loss of power or phase
        UnderVoltage = 0x00000008, // Voltage below threshold (Phase Loss)
        LowPowerFactor =  0x00000010, // Power Factor below threshold (can indicate miss-associated voltage and current inputs in three phase systems)
        OverCurrent = 0x00000020, // Current Input over threshold (out of measurement range)
        OverVoltage = 0x00000040, // Voltage Input over threshold (out of measurement range)
        MissingSensor = 0x00000080, // Sensor not connected
        Reserved1 = 0x00000100, // Reserved for future
        Reserved2 = 0x00000200, // Reserved for future
        Reserved3 = 0x00000400, // Reserved for future
        Reserved4 = 0x00000800, // Reserved for future
        Reserved5 = 0x00001000, // Reserved for future
        Reserved6 = 0x00002000, // Reserved for future
        Reserved7 = 0x00004000, // Reserved for future
        Reserved8 = 0x00008000, // Reserved for future
        OEM1_15 = 0x7FFF000 // Reserved for OEMs
    }

    [RegisterMapping(40121, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 67)]
    public class Meter1Info : Instance
    {
        //40121 1 C_SunSpec_DID uint16 N/A Value = 0x0001.Uniquely identifies this as a SunSpec Common Model Block
        [RegisterMapping(40121, RegisterType.UInt16)]
        public UInt16 DeviceId { get; set; }

        //40122 1 C_SunSpec_Length uint16 N/A 65 = Length of block in 16-bit registers
        [RegisterMapping(40122, RegisterType.UInt16)]
        public UInt16 Length { get; set; }

        //40123 16 C_Manufacturer String(32) N/A Meter manufacturer
        [RegisterMapping(40123, RegisterType.String, Size = 16)]
        public string Manufacturer { get; set; }

        //40139 16 C_Model String(32) N/A Meter model
        [RegisterMapping(40139, RegisterType.String, Size = 16)]
        public string Model { get; set; }

        //40155 8 C_Option String(16) N/A Export + Import, Production, consumption,
        [RegisterMapping(40155, RegisterType.String, Size = 8)]
        public string Option { get; set; }

        //40163 8 C_Version String(16) N/A Meter version
        [RegisterMapping(40163, RegisterType.String, Size = 8)]
        public string Version { get; set; }

        //40171 16 C_SerialNumber String(32) N/A Meter SN
        [RegisterMapping(40171, RegisterType.String, Size = 16)]
        public string SerialNumber { get; set; }

        //40187 1 C_DeviceAddress uint16 N/A Inverter Modbus ID
        [RegisterMapping(40187, RegisterType.UInt16)]
        public UInt16 DeviceAddress { get; set; }

        //40188 1 C_SunSpec_DID uint16 N/A Well-known value.Uniquely identifies this as a        
        [RegisterMapping(40188, RegisterType.UInt16)]
        public ModbusType ModbusType { get; set; }
    }


    //Current
    [RegisterMapping(40190, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 5)]
    public class Meter1Current : Instance
    {
        //40190 1 M_AC_Current int16 Amps AC Current(sum of active phases)
        [RegisterMapping(40190, RegisterType.ScaledInt16, RegisterUnit.Amps, ScalingFactorAddress = 40194)]
        public float Total { get; set; }

        //40191 1 M_AC_Current_A int16 Amps Phase A AC Current
        [RegisterMapping(40191, RegisterType.ScaledInt16, RegisterUnit.Amps, ScalingFactorAddress = 40194)]
        public float PhaseA { get; set; }

        //40192 1 M_AC_Current_B int16 Amps Phase B AC Current
        [RegisterMapping(40192, RegisterType.ScaledInt16, RegisterUnit.Amps, ScalingFactorAddress = 40194)]
        public float PhaseB { get; set; }

        //40193 1 M_AC_Current_C int16 Amps Phase C AC Current
        [RegisterMapping(40193, RegisterType.ScaledInt16, RegisterUnit.Amps, ScalingFactorAddress = 40194)]
        public float PhaseC { get; set; }

        //40194 1 M_AC_Current_S F int16 SF AC Current Scale Factor
    }

    //Voltage
    [RegisterMapping(40195, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 11)]
    public class Meter1Voltage : Instance
    {
        //Line to Neutral Voltage
        //40195 1 M_AC_Voltage_L N int16 Volts Line to Neutral AC Voltage(average of active
        [RegisterMapping(40195, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float AveragePhaseToNeutral { get; set; }
        //phases)
        //40196 1 M_AC_Voltage_A N int16 Volts Phase A to Neutral AC Voltage
        [RegisterMapping(40196, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float PhaseAToNeutral { get; set; }

        //40197 1 M_AC_Voltage_B N int16 Volts Phase B to Neutral AC Voltage
        [RegisterMapping(40197, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float PhaseBToNeutral { get; set; }

        //40198 1 M_AC_Voltage_C N int16 Volts Phase C to Neutral AC Voltage
        [RegisterMapping(40198, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float PhaseCToNeutral { get; set; }

        //Line to Line Voltage
        //40199 1 M_AC_Voltage_L L int16 Volts Line to Line AC Voltage(average of active
        //phases)
        [RegisterMapping(40199, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float AveragePhaseToLine { get; set; }

        //40200 1 M_AC_Voltage_A B int16 Volts Phase A to Phase B AC Voltage
        [RegisterMapping(40200, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float PhaseAToPhaseB { get; set; }

        //40201 1 M_AC_Voltage_B C int16 Volts Phase B to Phase C AC Voltage
        [RegisterMapping(40201, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float PhaseBToPhaseC { get; set; }

        //40202 1 M_AC_Voltage_C A int16 Volts Phase C to Phase A AC Voltage
        [RegisterMapping(40202, RegisterType.ScaledInt16, RegisterUnit.Volts, ScalingFactorAddress = 40203)]
        public float PhaseCToPhaseA { get; set; }

        //40203 1 M_AC_Voltage_S F int16 SF AC Voltage Scale Factor

        //Frequency
        //40204 1 M_AC_Freq int16 Herts AC Freque
        [RegisterMapping(40204, RegisterType.ScaledInt16, RegisterUnit.Hertz, ScalingFactorAddress = 40205)]
        public float ACFrequency { get; set; }
    }

    [RegisterMapping(40206, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 5)]
    public class Meter1RealPower : Instance
    {
        //40206 1 M_AC_Power int16 Watts Total Real Power(sum of active phases)
        [RegisterMapping(40206, RegisterType.ScaledInt16, RegisterUnit.Watts, ScalingFactorAddress = 40210)]
        public float Total { get; set; }

        //40207 1 M_AC_Power_A int16 Watts Phase A AC Real Power
        [RegisterMapping(40207, RegisterType.ScaledInt16, RegisterUnit.Watts, ScalingFactorAddress = 40210)]
        public float PhaseA { get; set; }

        //40208 1 M_AC_Power_B int16 Watts Phase B AC Real Power
        [RegisterMapping(40208, RegisterType.ScaledInt16, RegisterUnit.Watts, ScalingFactorAddress = 40210)]
        public float PhaseB { get; set; }

        //40209 1 M_AC_Power_C int16 Watts Phase C AC Real Power
        [RegisterMapping(40209, RegisterType.ScaledInt16, RegisterUnit.Watts, ScalingFactorAddress = 40210)]
        public float PhaseC { get; set; }

        //40210 1 M_AC_Power_SF int16 SF AC Real Power Scale Factor
    }

    [RegisterMapping(40211, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 5)]
    public class Meter1ApparentPower : Instance
    {
        //Apparent Power
        //40211 1 M_AC_VA int16 Volt- Amps Total AC Apparent Power(sum of active
        //phases)
        [RegisterMapping(40211, RegisterType.ScaledInt16, RegisterUnit.VoltAmps, ScalingFactorAddress = 40215)]
        public float Total { get; set; }

        //40212 1 M_AC_VA_A int16 Volt- Amps Phase A AC Apparent Power
        [RegisterMapping(40212, RegisterType.ScaledInt16, RegisterUnit.VoltAmps, ScalingFactorAddress = 40215)]
        public float PhaseA { get; set; }

        //40213 1 M_AC_VA_B int16 Volt- Amps Phase B AC Apparent Power
        [RegisterMapping(40213, RegisterType.ScaledInt16, RegisterUnit.VoltAmps, ScalingFactorAddress = 40215)]
        public float PhaseB { get; set; }

        //40214 1 M_AC_VA_C int16 Volt- Amps Phase C AC Apparent Power
        [RegisterMapping(40214, RegisterType.ScaledInt16, RegisterUnit.VoltAmps, ScalingFactorAddress = 40215)]
        public float PhaseC { get; set; }

        //40215 1 M_AC_VA_SF int16 SF AC Apparent Power Scale Factor
    }

    [RegisterMapping(40216, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 5)]
    public class Meter1ReactivePower : Instance
    {
        //Reactive Power
        //40216 1 M_AC_VAR int16 VAR Total AC Reactive Power(sum of active phases)
        [RegisterMapping(40216, RegisterType.ScaledInt16, RegisterUnit.VAR, ScalingFactorAddress = 40220)]
        public float Total { get; set; }

        //40217 1 M_AC_VAR_A int16 VAR Phase A AC Reactive Power
        [RegisterMapping(40217, RegisterType.ScaledInt16, RegisterUnit.VAR, ScalingFactorAddress = 40220)]
        public float PhaseA { get; set; }

        //40218 1 M_AC_VAR_B int16 VAR Phase B AC Reactive Power
        [RegisterMapping(40218, RegisterType.ScaledInt16, RegisterUnit.VAR, ScalingFactorAddress = 40220)]
        public float PhaseB { get; set; }

        //40219 1 M_AC_VAR_C int16 VAR Phase C AC Reactive Power
        [RegisterMapping(40219, RegisterType.ScaledInt16, RegisterUnit.VAR, ScalingFactorAddress = 40220)]
        public float PhaseC { get; set; }

        //40220 1 M_AC_VAR_SF int16 SF AC Reactive Power Scale Factor
    }

    [RegisterMapping(40221, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 5)]
    public class Meter1PowerFactor : Instance
    {
        //Power Factor
        //40221 1 M_AC_PF int16 % Average Power Factor(average of active
        //phases)
        [RegisterMapping(40221, RegisterType.ScaledInt16, RegisterUnit.Percent, ScalingFactorAddress = 40225)]
        public float Average { get; set; }

        //40222 1 M_AC_PF_A int16 % Phase A Power Factor
        [RegisterMapping(40222, RegisterType.ScaledInt16, RegisterUnit.Percent, ScalingFactorAddress = 40225)]
        public float PhaseA { get; set; }

        //40223 1 M_AC_PF_B int16 % Phase B Power Factor
        [RegisterMapping(40223, RegisterType.ScaledInt16, RegisterUnit.Percent, ScalingFactorAddress = 40225)]
        public float PhaseB { get; set; }

        //40224 1 M_AC_PF_C int16 % Phase C Power Factor
        [RegisterMapping(40224, RegisterType.ScaledInt16, RegisterUnit.Percent, ScalingFactorAddress = 40225)]
        public float PhaseC { get; set; }

        //40225 1 M_AC_PF_SF int16 SF AC Power Factor Scale Factor
    }

    [RegisterMapping(40226, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 172)]
    public class Meter1AccumulatedRealEnergy : Instance
    {
        //Real Energy
        //40226 2 M_Exported uint32 Watt- hours Total Exported Real Energy
        [RegisterMapping(40226, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float TotalExported { get; set; }

        //40228 2 M_Exported_A uint32 Watt- hours Phase A Exported Real Energy
        [RegisterMapping(40228, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float PhaseAExported { get; set; }

        //40230 2 M_Exported_B uint32 Watt- hours Phase B Exported Real Energy
        [RegisterMapping(40230, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float PhaseBExported { get; set; }

        //40232 2 M_Exported_C uint32 Watt- hours Phase C Exported Real Energy
        [RegisterMapping(40232, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float PhaseCExported { get; set; }

        //40234 2 M_Imported uint32 Watt- hours Total Imported Real Energy
        [RegisterMapping(40234, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float TotalImported { get; set; }

        //40236 2 M_Imported_A uint32 Watt- hours Phase A Imported Real Energy
        [RegisterMapping(40236, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float PhaseAImported { get; set; }

        //40238 2 M_Imported_B uint32 Watt- hours Phase B Imported Real Energy
        [RegisterMapping(40238, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float PhaseBImported { get; set; }

        //40240 2 M_Imported_C uint32 Watt- hours Phase C Imported Real Energy
        [RegisterMapping(40240, RegisterType.ScaledUInt32, RegisterUnit.WattHours, ScalingFactorAddress = 40242)]
        public float PhaseCImported { get; set; }

        //40242 1 M_Energy_W_SF int16 SF Real Energy Scale Factor
    }

    [RegisterMapping(40243, RegisterType.Object, Addressing = RegisterAddressing.Base0)]
    public class Meter1AccumulatedApparentEnergy : Instance
    {
        //Apparent Energy
        //40243 2 M_Exported_VA uint32 VA-hours Total Exported Apparent Energy
        [RegisterMapping(40243, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float TotalExported { get; set; }

        //40245 2 M_Exported_VA_ A uint32 VA-hours Phase A Exported Apparent Energy
        [RegisterMapping(40245, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float PhaseAExported { get; set; }

        //40247 2 M_Exported_VA_ B uint32 VA-hours Phase B Exported Apparent Energy
        [RegisterMapping(40247, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float PhaseBExported { get; set; }

        //40249 2 M_Exported_VA_ C uint32 VA-hours Phase C Exported Apparent Energy
        [RegisterMapping(40249, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float PhaseCExported { get; set; }

        //40251 2 M_Imported_VA uint32 VA-hours Total Imported Apparent Energy
        [RegisterMapping(40251, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float TotalImported { get; set; }

        //40253 2 M_Imported_VA_ A uint32 VA-hours Phase A Imported Apparent Energy
        [RegisterMapping(40253, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float PhaseAImported { get; set; }

        //40255 2 M_Imported_VA_ B uint32 VA-hours Phase B Imported Apparent Energy
        [RegisterMapping(40255, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float PhaseBImported { get; set; }

        //40257 2 M_Imported_VA_ C uint32 VA-hours Phase C Imported Apparent Energy
        [RegisterMapping(40257, RegisterType.ScaledUInt32, RegisterUnit.VAHours, ScalingFactorAddress = 40259)]
        public float PhaseCImported { get; set; }

        //40259 1 M_Energy_VA_S F int16 SF Apparent Energy Scale Factor
    }

    [RegisterMapping(40260, RegisterType.Object, Addressing = RegisterAddressing.Base0)]
    public class Meter1AccumulatedReactiveEnergy : Instance
    {
        //Reactive Energy
        //40260 2 M_Import_VARh_ Q1 uint32 VAR-hours Quadrant 1: Total Imported Reactive Energy
        [RegisterMapping(40260, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant1TotalImported { get; set; }

        //40262 2 M_Import_VARh_ Q1A uint32 VAR-hours Phase A - Quadrant 1: Imported Reactive Energy
        [RegisterMapping(40262, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant1PhaseAImported { get; set; }

        //40264 2 M_Import_VARh_ Q1B uint32 VAR-hours Phase B- Quadrant 1: Imported Reactive Energy
        [RegisterMapping(40264, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant1PhaseBImported { get; set; }

        //40266 2 M_Import_VARh_ Q1C uint32 VAR-hours Phase C- Quadrant 1: Imported Reactive Energy
        [RegisterMapping(40266, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant1PhaseCImported { get; set; }

        //40268 2 M_Import_VARh_ Q2 uint32 VAR-hours Quadrant 2: Total Imported Reactive Energy
        [RegisterMapping(40268, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant2TotalImported { get; set; }

        //40270 2 M_Import_VARh_ Q2A uint32 VAR-hours Phase A - Quadrant 2: Imported Reactive Energy
        [RegisterMapping(40270, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant2PhaseAImported { get; set; }

        //40272 2 M_Import_VARh_ Q2B uint32 VAR-hours Phase B- Quadrant 2: Imported Reactive Energy
        [RegisterMapping(40272, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant2PhaseBImported { get; set; }

        //40274 2 M_Import_VARh_ Q2C uint32 VAR-hours Phase C- Quadrant 2: Imported Reactive Energy
        [RegisterMapping(40274, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant2PhaseCImported { get; set; }

        //40276 2 M_Export_VARh_ Q3 uint32 VAR-hours Quadrant 3: Total Exported Reactive Energy
        [RegisterMapping(40276, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant3TotalExported { get; set; }

        //40278 2 M_Export_VARh_ Q3A uint32 VAR-hours Phase A - Quadrant 3: Exported Reactive Energy
        [RegisterMapping(40278, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant3PhaseAExported { get; set; }

        //40280 2 M_Export_VARh_ Q3B uint32 VAR-hours Phase B- Quadrant 3: Exported Reactive Energy
        [RegisterMapping(40280, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant3PhaseBExported { get; set; }

        //40282 2 M_Export_VARh_ Q3C uint32 VAR-hours Phase C- Quadrant 3: Exported Reactive Energy
        [RegisterMapping(40282, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant3PhaseCExported { get; set; }

        //40284 2 M_Export_VARh_ Q4 uint32 VAR-hours Quadrant 4: Total Exported Reactive Energy
        [RegisterMapping(40284, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant4TotalExported { get; set; }

        //40286 2 M_Export_VARh_ Q4A uint32 VAR-hours Phase A - Quadrant 4: Exported Reactive Energy
        [RegisterMapping(40286, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant4PhaseAExported { get; set; }

        //40288 2 M_Export_VARh_ Q4B uint32 VAR-hours Phase B- Quadrant 4: Exported Reactive Energy
        [RegisterMapping(40288, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant4PhaseBExported { get; set; }

        //40290 2 M_Export_VARh_ Q4C uint32 VAR-hours Phase C- Quadrant 4: Exported Reactive Energy
        [RegisterMapping(40290, RegisterType.ScaledUInt32, RegisterUnit.VARHours, ScalingFactorAddress = 40292)]
        public float Quadrant4PhaseCExported { get; set; }

        //40292 1 M_Energy_VAR_ SF int16 SF Reactive Energy Scale Factor
    }

    [RegisterMapping(40190, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 103)]
    public class Meter1Readings : Instance
    {
        [RegisterMapping(40190, RegisterType.Object)]
        public Meter1Current Current { get; set; }

        [RegisterMapping(40195, RegisterType.Object)]
        public Meter1Voltage Voltages { get; set; }

        [RegisterMapping(40206, RegisterType.Object)]
        public Meter1RealPower RealPower { get; set; }

        [RegisterMapping(40211, RegisterType.Object)]
        public Meter1ApparentPower ApparentPower { get; set; }

        [RegisterMapping(40216, RegisterType.Object)]
        public Meter1ReactivePower ReactivePower { get; set; }

        [RegisterMapping(40221, RegisterType.Object)]
        public Meter1PowerFactor PowerFactor { get; set; }

        [RegisterMapping(40226, RegisterType.Object)]
        public Meter1AccumulatedRealEnergy AccumulatedRealEnergy { get; set; }

        [RegisterMapping(40243, RegisterType.Object)]
        public Meter1AccumulatedApparentEnergy AccumulatedApparentEnergy { get; set; }

        [RegisterMapping(40260, RegisterType.Object)]
        public Meter1AccumulatedReactiveEnergy AccumulatedReactiveEnergy { get; set; }
    }

    [RegisterMapping(40121, RegisterType.Object, Addressing = RegisterAddressing.Base0, Size = 172)]
    public class Meter1 : Instance
    {
        [RegisterMapping(40121, RegisterType.Object)]
        public Meter1Info Information { get; set; }

        [RegisterMapping(40190, RegisterType.Object)]
        public Meter1Readings Readings { get; set; }

        //Events
        //40293 2 M_Events uint32 Flags See M_EVENT_ flags. 0 = nts.
        [RegisterMapping(40293, RegisterType.UInt32)]
        public MeterEvents Events { get; set; }
    }
}
