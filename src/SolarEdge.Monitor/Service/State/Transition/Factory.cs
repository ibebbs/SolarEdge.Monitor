namespace SolarEdge.Monitor.Service.State.Transition
{
    public interface IFactory
    {
        ITransition ToConnecting();
        ITransition ToConnected(IInverter inverter);
        ITransition ToDisconnected();
        ITransition ToDisconnecting(IInverter inverter);
    }

    public class Factory : IFactory
    {
        public ITransition ToConnected(IInverter inverter)
        {
            return new ToConnected(inverter);
        }

        public ITransition ToConnecting()
        {
            return new ToConnecting();
        }

        public ITransition ToDisconnected()
        {
            return new ToDisconnected();
        }

        public ITransition ToDisconnecting(IInverter inverter)
        {
            return new ToDisconnecting(inverter);
        }
    }
}
