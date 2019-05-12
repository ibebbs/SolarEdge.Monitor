namespace SolarEdge.Monitor.Service.State.Transition
{
    public class ToDisconnecting : ITransition
    {
        public ToDisconnecting(IInverter inverter)
        {
            Inverter = inverter;
        }

        public IInverter Inverter { get; }
    }
}
