using System;
using Evidos.EventSourcing.Domain.Enums;

namespace Evidos.EventSourcing.Domain.Core.User
{
    public class UserUpdatedEvent : DomainEventBase
    {
        public string EmailAddress { get; }
        public UserStatus UserStatus { get; }
        
        internal UserUpdatedEvent(Guid userId, long userVersion, string emailAddress, UserStatus userStatus) 
            : base(userId, userVersion)
        {
            EmailAddress = emailAddress;
            UserStatus = userStatus;
        }
    }
}
