using System;

namespace Evidos.EventSourcing.Domain.Core.User
{
    public class UserDeletedEvent : DomainEventBase
    {
        internal UserDeletedEvent(Guid userId, long userVersion) : base(userId, userVersion)
        {
        }
    }
}
