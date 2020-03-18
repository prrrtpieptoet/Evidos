using System;
using System.Reflection;
using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.Core;
using Evidos.EventSourcing.Domain.Core.Abstractions;
using Evidos.EventSourcing.Domain.PubSub.Abstractions;
using Evidos.EventSourcing.Domain.Storage.Abstractions;
using Evidos.EventSourcing.Domain.Storage.EventStore.Abstractions;
using Evidos.EventSourcing.Domain.Storage.EventStore.Exceptions;

namespace Evidos.EventSourcing.Domain.Storage.EventStore
{
    public class EventStoreAggregateRepository<TAggregate> : IAggregateRepository<TAggregate>
        where TAggregate : AggregateBase, IAggregate
    {
        private readonly IEventStore _eventStore;
        private readonly IPublisher _publisher;

        public EventStoreAggregateRepository(IEventStore eventStore, IPublisher publisher)
        {
            _eventStore = eventStore;
            _publisher = publisher;
        }

        public async Task<TAggregate> GetByIdAsync(Guid id)
        {
            try
            {
                var aggregate = CreateEmptyAggregate();

                foreach (var @event in await _eventStore.ReadEventsAsync(id))
                {
                    aggregate.ApplyEvent(@event.DomainEvent, @event.EventNumber);
                }
                return aggregate;
            }
            catch (EventStoreAggregateNotFoundException)
            {
                return null;
            }
        }

        public async Task SaveAsync(TAggregate aggregate)
        {
            foreach (var domainEvent in aggregate.GetUncommittedEvents())
            {
                await _eventStore.AppendEventAsync(domainEvent);
                await _publisher.PublishAsync((dynamic) domainEvent);
            }

            aggregate.ClearUncommittedEvents();
        }

        public async Task DeleteAsync(TAggregate aggregate)
        {
            foreach (var domainEvent in aggregate.GetUncommittedEvents())
            {
                await _eventStore.DeleteEventsAsync(aggregate.Id);
                await _publisher.PublishAsync((dynamic) domainEvent);
            }

            aggregate.ClearUncommittedEvents();
        }

        private TAggregate CreateEmptyAggregate()
        {
            return (TAggregate)typeof(TAggregate)
                .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, 
                    null, new Type[0], new ParameterModifier[0])
                .Invoke(new object[0]);
        }
    }
}
