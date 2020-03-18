using System.Collections.Generic;
using Evidos.EventSourcing.Domain.Core.Abstractions;

namespace Evidos.EventSourcing.Domain.Storage.EventStore.Abstractions
{
    internal interface IEventSourcingAggregate
    {
        long Version { get; }
        void ApplyEvent(IDomainEvent domainEvent, long version);
        IEnumerable<IDomainEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}
