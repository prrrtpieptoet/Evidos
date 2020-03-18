using System.Collections.Generic;

namespace Evidos.EventSourcing.Domain.Core.Abstractions
{
    internal interface IEventSourcingAggregate
    {
        long Version { get; }
        void ApplyEvent(IDomainEvent domainEvent, long version);
        IEnumerable<IDomainEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}
