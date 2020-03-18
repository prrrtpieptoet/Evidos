using System;
using Evidos.EventSourcing.Domain.Core.Abstractions;

namespace Evidos.EventSourcing.Domain.Core
{
    public abstract class DomainEventBase : IDomainEvent
    {
        public Guid EventId { get; }
        public Guid AggregateId { get; }
        public long AggregateVersion { get; }

        protected DomainEventBase()
        {
            EventId = Guid.NewGuid();
        }
        
        protected DomainEventBase(Guid aggregateId) : this()
        {
            AggregateId = aggregateId;
        }

        protected DomainEventBase(Guid aggregateId, long aggregateVersion) : this(aggregateId)
        {
            AggregateVersion = aggregateVersion;
        }
    }
}
