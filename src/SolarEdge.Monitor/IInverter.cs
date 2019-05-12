using System.Threading.Tasks;

namespace SolarEdge.Monitor
{
    public delegate void ConnectionEvent(object sender);

    public interface IInverter
    {
        event ConnectionEvent Connected;
        event ConnectionEvent Disconnected;

        Task<bool> ConnectAsync();
        Task DisconnectAsync();

        Task<float> GetCurrentGenerationAsync();

        Task<T> Get<T>() where T : Inverter.Model.Instance;
    }
}
