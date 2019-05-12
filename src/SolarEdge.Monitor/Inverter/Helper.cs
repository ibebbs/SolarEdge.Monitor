using EasyModbus;
using System;
using System.Linq;

namespace SolarEdge.Monitor.Inverter
{
    public static class Helper
    {
        public static T Read<T>(this ModbusClient client)
        {
            var typeRegisterMapping = typeof(T)
                .GetCustomAttributes(typeof(Model.RegisterMappingAttribute), false)
                .OfType<Model.RegisterMappingAttribute>()
                .Where(registerMapping => registerMapping.Type == Model.RegisterType.Object)
                .First();

            var address = typeRegisterMapping.Addressing == Model.RegisterAddressing.Base1
                ? (int) typeRegisterMapping.Address - 1
                : (int) typeRegisterMapping.Address;

            // TODO: Remove size constraint of 125
            var memory = client.ReadHoldingRegistersRaw(address, Math.Min((int)typeRegisterMapping.Size, 125)).AsMemory().Slice(9);
            
            return Model.Reader.Default.Read<T>(memory);
        }
    }
}
