using System;

namespace Evidos.EventSourcing.Domain.Core.Abstractions
{
    public interface IAggregate
    {
        Guid Id { get; }
    }
}

