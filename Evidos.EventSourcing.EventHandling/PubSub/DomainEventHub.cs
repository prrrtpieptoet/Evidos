using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.PubSub.Abstractions;

namespace Evidos.EventSourcing.EventHandling.PubSub
{
    public class DomainEventHub : IDisposable, IPublisher, ISubscriber
    {
        private static AsyncLocal<Dictionary<Type, List<object>>> handlers = new AsyncLocal<Dictionary<Type, List<object>>>();

        public Dictionary<Type, List<object>> Handlers
        {
            get => handlers.Value ?? (handlers.Value = new Dictionary<Type, List<object>>());
        }

        public DomainEventHub()
        {
        }

        public void Subscribe<T>(Action<T> handler)
        {
            GetHandlersOf<T>().Add(handler);
        }

        public void Subscribe<T>(Func<T, Task> handler)
        {
            GetHandlersOf<T>().Add(handler);
        }

        public async Task PublishAsync<T>(T publishedEvent)
        {
            foreach (var handler in GetHandlersOf<T>())
            {
                try
                {
                    switch (handler)
                    {
                        case Action<T> action:
                            action(publishedEvent);
                            break;
                        case Func<T, Task> action:
                            await action(publishedEvent);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    // Log?
                }
            }
        }

        public void Dispose()
        {
            foreach (var handlersOfT in Handlers.Values)
            {
                handlersOfT.Clear();
            }
            Handlers.Clear();
        }

        private ICollection<object> GetHandlersOf<T>()
        {
            List<object> ret;

            if (!Handlers.TryGetValue(typeof(T), out ret) || ret == null)
            {
                Handlers[typeof(T)] = new List<object>();
            }

            return Handlers[typeof(T)];
        }
    }
}
