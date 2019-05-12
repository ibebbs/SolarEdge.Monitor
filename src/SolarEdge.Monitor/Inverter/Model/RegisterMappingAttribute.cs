using System;

namespace SolarEdge.Monitor.Inverter.Model
{
    public enum RegisterType
    {
        Object,
        UInt16,
        UInt32,
        Int16,
        Int32,
        ScaledUInt16,
        ScaledUInt32,
        ScaledInt16,
        ScaledInt32,
        Acc32,
        String
    }

    public enum RegisterUnit
    {
        None,
        Amps,
        VoltAmps,
        Volts,
        Watts,
        Hertz,
        VA,
        VAHours,
        VAR,
        VARHours,
        Percent,
        WattHours,
        Celcius
    }

    public enum RegisterAddressing
    {
        Base1,
        Base0
    }

    public class RegisterMappingAttribute : Attribute
    {
        public RegisterMappingAttribute(uint address, RegisterType type = RegisterType.Object, RegisterUnit unit = RegisterUnit.None)
        {
            Address = address;
            Type = type;
            Unit = unit;
        }

        public uint Address { get; }
        public RegisterType Type { get; }
        public RegisterUnit Unit { get; }
        public RegisterAddressing Addressing { get; set; }
        public string Name { get; set; }
        public uint ScalingFactorAddress { get; set; }
        public uint Size { get; set; }
    }
}
