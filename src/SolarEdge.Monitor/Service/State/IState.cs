using System;

namespace SolarEdge.Monitor.Service.State
{
    public interface IState
    {
        IObservable<ITransition> Enter();
    }
}
