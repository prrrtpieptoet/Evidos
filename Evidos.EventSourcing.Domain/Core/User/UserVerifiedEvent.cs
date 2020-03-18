using System;
using Evidos.EventSourcing.Domain.Enums;

namespace Evidos.EventSourcing.Domain.Core.User
{
    public class UserVerifiedEvent : DomainEventBase
    {
        public UserStatus UserStatus { get; }

        internal UserVerifiedEvent(Guid userId, long userVersion, UserStatus userStatus) : base(userId, userVersion)
        {
            UserStatus = userStatus;
        }
    }
}
