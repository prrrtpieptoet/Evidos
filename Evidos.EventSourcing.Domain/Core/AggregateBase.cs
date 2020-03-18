using System;
using System.Collections.Generic;
using System.Linq;
using Evidos.EventSourcing.Domain.Core.Abstractions;
using Evidos.EventSourcing.Domain.Storage.EventStore.Abstractions;

namespace Evidos.EventSourcing.Domain.Core
{
    public abstract class AggregateBase : IAggregate, IEventSourcingAggregate
    {
        public Guid Id { get; protected set;  }
        public long Version { get; private set; }

        protected readonly ICollection<IDomainEvent> UncommittedEvents = new LinkedList<IDomainEvent>();

        public void ApplyEvent(IDomainEvent domainEvent, long version)
        {
            if (UncommittedEvents.Any(x => Equals(x.EventId, domainEvent.EventId)))
                return;

            ((dynamic)this).Apply((dynamic)domainEvent);
            Version = version;
        }

        public void ClearUncommittedEvents()
        {
            UncommittedEvents.Clear();
        }

        public IEnumerable<IDomainEvent> GetUncommittedEvents()
        {
            return UncommittedEvents.AsEnumerable();
        }
    }
}
