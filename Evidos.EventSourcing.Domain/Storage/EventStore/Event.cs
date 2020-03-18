using Evidos.EventSourcing.Domain.Core.Abstractions;

namespace Evidos.EventSourcing.Domain.Storage.EventStore
{
    public class Event
    {
        public Event(IDomainEvent domainEvent, long eventNumber)
        {
            DomainEvent = domainEvent;
            EventNumber = eventNumber;
        }

        public long EventNumber { get; }

        public IDomainEvent DomainEvent { get; }
    }
}
