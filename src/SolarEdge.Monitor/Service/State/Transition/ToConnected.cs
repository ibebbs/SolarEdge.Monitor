namespace SolarEdge.Monitor.Service.State.Transition
{
    public class ToConnected : ITransition
    {
        public ToConnected(IInverter inverter)
        {
            Inverter = inverter;
        }

        public IInverter Inverter { get; }
    }
}
