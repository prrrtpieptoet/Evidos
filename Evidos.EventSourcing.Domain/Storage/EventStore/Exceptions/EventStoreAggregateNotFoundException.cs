using System;

namespace Evidos.EventSourcing.Domain.Storage.EventStore.Exceptions
{
    [Serializable]
    public class EventStoreAggregateNotFoundException : EventStoreException
    {
        public EventStoreAggregateNotFoundException() { }
        public EventStoreAggregateNotFoundException(string message) : base(message) { }
        public EventStoreAggregateNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected EventStoreAggregateNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
