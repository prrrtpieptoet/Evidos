using System;
using Evidos.EventSourcing.Domain.Enums;

namespace Evidos.EventSourcing.Domain.Core.User
{
    public class UserSubmittedEvent : DomainEventBase
    {
        public string EmailAddress { get; }
        public string Password { get; }
        public UserStatus UserStatus { get; }
        public DateTime DateTimeCreated { get; }
        
        internal UserSubmittedEvent(Guid userId, long userVersion, 
            string emailAddress, string password, UserStatus userStatus, DateTime dateTimeCreated) 
            : base(userId, userVersion)
        {
            EmailAddress = emailAddress;
            Password = password;
            UserStatus = userStatus;
            DateTimeCreated = dateTimeCreated;
        }
    }
}
