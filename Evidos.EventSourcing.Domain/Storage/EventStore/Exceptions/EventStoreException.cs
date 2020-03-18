using System;

namespace Evidos.EventSourcing.Domain.Storage.EventStore.Exceptions
{
    [Serializable]
    public class EventStoreException : Exception
    {
        public EventStoreException() { }
        public EventStoreException(string message) : base(message) { }
        public EventStoreException(string message, Exception inner) : base(message, inner) { }
        protected EventStoreException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
