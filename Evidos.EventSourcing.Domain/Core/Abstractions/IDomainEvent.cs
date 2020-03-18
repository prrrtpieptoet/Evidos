using System;

namespace Evidos.EventSourcing.Domain.Core.Abstractions
{
    public interface IDomainEvent
    {
        Guid EventId { get; }

        Guid AggregateId { get; }
    }
}
