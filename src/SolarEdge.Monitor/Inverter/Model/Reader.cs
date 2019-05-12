using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SolarEdge.Monitor.Inverter.Model
{
    public class Reader
    {
        public static readonly Reader Default = new Reader();

        private static readonly MethodInfo GenericReadMethod = typeof(Reader).GetMethod(nameof(Read), BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo GenericCastMethod = typeof(Reader).GetMethod(nameof(Cast), BindingFlags.Static | BindingFlags.NonPublic);

        private static T Cast<T>(object o)
        {
            return (T)o;
        }

        private Memory<byte> GetMemory(Memory<byte> memory, uint baseAddress, uint registerAddress, int? length = null)
        {
            if (length.HasValue)
            {
                return memory.Slice((int)(registerAddress - baseAddress) * 2, length.Value);
            }
            else
            {
                return memory.Slice((int)(registerAddress - baseAddress) * 2);
            }
        }

        private UInt32 ReadAcc32(Memory<byte> memory, uint baseAddress, uint registerAddress)
        {
            var span = GetMemory(memory, baseAddress, registerAddress, 4).ToArray().Reverse().ToArray();

            return BitConverter.ToUInt32(span, 0);
        }

        private Int16 ReadInt16(Memory<byte> memory, uint baseAddress, uint registerAddress)
        {
            var span = GetMemory(memory, baseAddress, registerAddress, 2).ToArray().Reverse().ToArray();

            return BitConverter.ToInt16(span, 0);
        }

        private Int32 ReadInt32(Memory<byte> memory, uint baseAddress, uint registerAddress)
        {
            var span = GetMemory(memory, baseAddress, registerAddress, 4).ToArray().Reverse().ToArray();

            return BitConverter.ToInt32(span, 0);
        }

        private object ReadObject(Memory<byte> memory, uint baseAddress, PropertyInfo property, RegisterMappingAttribute propertyRegisterMapping)
        {
            var span = GetMemory(memory, baseAddress, propertyRegisterMapping.Address);

            var result = GenericReadMethod.MakeGenericMethod(property.PropertyType).Invoke(this, new object[] { span });

            return result;
        }

        private float ReadScaledInt16(Memory<byte> memory, uint baseAddress, RegisterMappingAttribute propertyRegisterMapping)
        {
            var value = (Int16)ReadInt16(memory, baseAddress, propertyRegisterMapping.Address);
            var scale = (Int16)ReadInt16(memory, baseAddress, propertyRegisterMapping.ScalingFactorAddress);

            return Convert.ToSingle(value * Math.Pow(10, scale));
        }

        private float ReadScaledInt32(Memory<byte> memory, uint baseAddress, RegisterMappingAttribute propertyRegisterMapping)
        {
            var value = (Int32)ReadInt32(memory, baseAddress, propertyRegisterMapping.Address);
            var scale = (Int16)ReadInt16(memory, baseAddress, propertyRegisterMapping.ScalingFactorAddress);

            return Convert.ToSingle(value * Math.Pow(10, scale));
        }

        private float ReadScaledUInt16(Memory<byte> memory, uint baseAddress, RegisterMappingAttribute propertyRegisterMapping)
        {
            var value = (UInt16)ReadUInt16(memory, baseAddress, propertyRegisterMapping.Address);
            var scale = (Int16)ReadInt16(memory, baseAddress, propertyRegisterMapping.ScalingFactorAddress);

            return Convert.ToSingle(value * Math.Pow(10, scale));
        }

        private float ReadScaledUInt32(Memory<byte> memory, uint baseAddress, RegisterMappingAttribute propertyRegisterMapping)
        {
            var value = (UInt32)ReadUInt16(memory, baseAddress, propertyRegisterMapping.Address);
            var scale = (Int16)ReadInt16(memory, baseAddress, propertyRegisterMapping.ScalingFactorAddress);

            return Convert.ToSingle(value * Math.Pow(10, scale));
        }

        private string ReadString(Memory<byte> memory, uint baseAddress, RegisterMappingAttribute propertyRegisterMapping)
        {
            var span = GetMemory(memory, baseAddress, propertyRegisterMapping.Address, (int)propertyRegisterMapping.Size).Span;

            return Encoding.UTF8.GetString(span).TrimEnd((char)0x00);
        }

        private UInt16 ReadUInt16(Memory<byte> memory, uint baseAddress, uint registerAddress)
        {
            var span = GetMemory(memory, baseAddress, registerAddress, 2).ToArray().Reverse().ToArray();

            return BitConverter.ToUInt16(span, 0);
        }

        private UInt32 ReadUInt32(Memory<byte> memory, uint baseAddress, uint registerAddress)
        {
            var span = GetMemory(memory, baseAddress, registerAddress, 4).ToArray().Reverse().ToArray();

            return BitConverter.ToUInt32(span, 0);
        }

        private object ReadValue(Memory<byte> memory, uint baseAddress, PropertyInfo property, RegisterMappingAttribute registerMapping)
        {
            switch (registerMapping.Type)
            {
                case RegisterType.Acc32: return ReadAcc32(memory, baseAddress, registerMapping.Address);
                case RegisterType.Int16: return ReadInt16(memory, baseAddress, registerMapping.Address);
                case RegisterType.Int32: return ReadInt32(memory, baseAddress, registerMapping.Address);
                case RegisterType.Object: return ReadObject(memory, baseAddress, property, registerMapping);
                case RegisterType.ScaledInt16: return ReadScaledInt16(memory, baseAddress, registerMapping);
                case RegisterType.ScaledInt32: return ReadScaledInt32(memory, baseAddress, registerMapping);
                case RegisterType.ScaledUInt16: return ReadScaledUInt16(memory, baseAddress, registerMapping);
                case RegisterType.ScaledUInt32: return ReadScaledUInt32(memory, baseAddress, registerMapping);
                case RegisterType.String: return ReadString(memory, baseAddress, registerMapping);
                case RegisterType.UInt16: return ReadUInt16(memory, baseAddress, registerMapping.Address);
                case RegisterType.UInt32: return ReadUInt32(memory, baseAddress, registerMapping.Address);
                default: throw new ArgumentException("Unknown register type");
            }
        }

        private T ReadRegister<T>(Memory<byte> memory, uint baseAddress, T instance, PropertyInfo property, RegisterMappingAttribute registerMapping)
        {
            var value = ReadValue(memory, baseAddress, property, registerMapping);

            var cast = GenericCastMethod.MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { value });

            property.SetValue(instance, cast);

            return instance;
        }

        public T Read<T>(Memory<byte> span)
        {
            var typeRegisterMapping = typeof(T)
                .GetCustomAttributes(typeof(RegisterMappingAttribute), false)
                .OfType<RegisterMappingAttribute>()
                .First();

            return typeof(T)
                .GetProperties()
                .SelectMany(property => property
                    .GetCustomAttributes(typeof(RegisterMappingAttribute), false)
                    .OfType<RegisterMappingAttribute>()
                    .Select(propertyRegisterMapping => new { Property = property, RegisterMapping = propertyRegisterMapping }))
                .Aggregate(Activator.CreateInstance<T>(), (instance, tuple) => ReadRegister(span, typeRegisterMapping.Address, instance, tuple.Property, tuple.RegisterMapping));
        }
    }
}
