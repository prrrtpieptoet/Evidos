using System;
using System.Threading.Tasks;

namespace Evidos.EventSourcing.Domain.PubSub.Abstractions
{
    public interface ISubscriber
    {
        void Subscribe<T>(Action<T> handler);

        void Subscribe<T>(Func<T, Task> handler);
    }
}
