using System;

namespace Evidos.EventSourcing.Query.Abstractions
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
