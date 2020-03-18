using System;

namespace Evidos.EventSourcing.Query.Exceptions
{
    [Serializable]
    public class EntityRepositoryException : Exception
    {
        public EntityRepositoryException()
        {
        }

        public EntityRepositoryException(string message) : base(message)
        {
        }

        public EntityRepositoryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EntityRepositoryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
