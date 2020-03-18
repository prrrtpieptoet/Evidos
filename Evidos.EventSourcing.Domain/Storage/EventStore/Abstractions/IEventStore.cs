using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.Core.Abstractions;

namespace Evidos.EventSourcing.Domain.Storage.EventStore.Abstractions
{
    public interface IEventStore
    {
        Task<IEnumerable<Event>> ReadEventsAsync(Guid aggregateId);

        Task AppendEventAsync(IDomainEvent domainEvent);

        Task AppendEventsAsync(LinkedList<IDomainEvent> domainEvents);
        
        Task DeleteEventsAsync(Guid aggregateId);
    }
}
