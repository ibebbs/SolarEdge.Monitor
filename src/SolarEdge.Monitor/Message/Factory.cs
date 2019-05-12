using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace SolarEdge.Monitor.Message
{
    public interface IFactory
    {
        string Create(float dcPowerGeneration);

        string Create(Inverter.Model.Inverter value);

        string Create<T>(T value) where T : Inverter.Model.Instance;
    }

    public class Factory : IFactory
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Converters = new[] { new StringEnumConverter() }
        };

        private static readonly JsonSerializer Serializer = JsonSerializer.Create(SerializerSettings);

        public string Create(float dcPowerGeneration)
        {
            var message = new Instance { DcPowerGeneration = dcPowerGeneration };

            string result = JsonConvert.SerializeObject(message);

            return result;
        }

        public string Create(Inverter.Model.Inverter value)
        {
            using (var text = new StringWriter())
            {
                using (var json = new JsonTextWriter(text))
                {
                    Serializer.Serialize(json, value);

                    return text.ToString();
                }
            }
        }

        public string Create<T>(T value)
            where T : Inverter.Model.Instance
        {
            using (var text = new StringWriter())
            {
                using (var json = new JsonTextWriter(text))
                {
                    Serializer.Serialize(json, value);

                    return text.ToString();
                }
            }
        }
    }
}
