using EasyModbus;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Inverter
{
    public class Instance : IInverter
    {
        private const int SUNSPEC_I_COMMON_BLOCK_START = 40000;
        private const int SUNSPEC_I_MODEL_BLOCK_START = 40069;
        private const int SUNSPEC_M_COMMON_BLOCK_START = 40121;
        private const int SUNSPEC_M_MODEL_BLOCK_START = 40188;

        private const int SOLAR_EDGE_DC_POWER = 40100;

        private readonly string _address;
        private readonly int _port;

        private ModbusClient _modbusClient;

        public event ConnectionEvent Connected;
        public event ConnectionEvent Disconnected;

        public Instance(string address, int port)
        {
            _address = address;
            _port = port;
        }

        private void ConnectionChanged(object sender)
        {
            if (_modbusClient.Connected)
            {
                Connected?.Invoke(this);
            }
            else
            {
                Connected?.Invoke(this);
            }
        }

        public async Task<bool> ConnectAsync()
        {
            if (_modbusClient != null)
            {
                throw new InvalidOperationException("Already connected");
            }

            _modbusClient = new ModbusClient(_address, _port);
            _modbusClient.connectedChanged += ConnectionChanged;

            try
            {
                await _modbusClient.ConnectAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task DisconnectAsync()
        {
            _modbusClient.Disconnect();
            _modbusClient = null;

            return Task.CompletedTask;
        }

        public Task<float> GetCurrentGenerationAsync()
        {
            if (_modbusClient == null)
            {
                throw new InvalidOperationException("Not yet connected. Call ConnectAsync first.");
            }

            //var data = _modbusClient.ReadHoldingRegistersRaw(SUNSPEC_I_MODEL_BLOCK_START, 39);

            //Console.Write(string.Join(", ", data.Select(b => b.ToString())));

            var response = _modbusClient.ReadHoldingRegisters(SOLAR_EDGE_DC_POWER, 2);

            var power = response[0] * Math.Pow(10, response[1]);

            return Task.FromResult(Convert.ToSingle(power));
        }

        public Task<T> Get<T>() where T : Model.Instance
        {
            if (_modbusClient == null)
            {
                throw new InvalidOperationException("Not yet connected. Call ConnectAsync first.");
            }

            var result = _modbusClient.Read<T>();

            return Task.FromResult(result);
        }
    }
}
