using System;
using Evidos.EventSourcing.Domain.Enums;

namespace Evidos.EventSourcing.Domain.Core.User
{
    public class User : AggregateBase
    {
        private string EmailAddress { get; set; }
        private string Password { get; set; }
        private UserStatus UserStatus { get; set; }
        private DateTime DateTimeCreated { get; set; }
        
        private User()
        {
        }

        public User(Guid userId, string emailAddress)
        {
            if (userId == Guid.Empty) 
                throw new ArgumentException(nameof(userId));

            if (string.IsNullOrWhiteSpace(emailAddress)) 
                throw new ArgumentNullException(nameof(emailAddress));
            
            var userSubmittedEvent = new UserSubmittedEvent(userId, Version, 
                emailAddress, string.Empty, UserStatus.VerificationPending, DateTime.UtcNow);

            ApplyEvent(userSubmittedEvent, Version + 1);
            UncommittedEvents.Add(userSubmittedEvent);
        }

        public void UpdateEmailAddress(Guid userId, string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentException(nameof(emailAddress));
            }
            
            var userUpdatedEvent = new UserUpdatedEvent(
                userId, Version, emailAddress, UserStatus.VerificationPending);

            ApplyEvent(userUpdatedEvent, Version + 1);
            UncommittedEvents.Add(userUpdatedEvent);
        }
        
        public void Delete(Guid userId)
        {
            var userDeletedEvent = new UserDeletedEvent(userId, Version);

            ApplyEvent(userDeletedEvent, Version + 1);
            UncommittedEvents.Add(userDeletedEvent);
        }
        
        public void VerifyEmailAddress(Guid userId)
        {
            var userVerifiedEvent = new UserVerifiedEvent(
                userId, Version, UserStatus.Verified);

            ApplyEvent(userVerifiedEvent, Version + 1);
            UncommittedEvents.Add(userVerifiedEvent);
        }

        internal void Apply(UserSubmittedEvent userSubmittedEvent)
        {
            Id = userSubmittedEvent.AggregateId;
            EmailAddress = userSubmittedEvent.EmailAddress;
            Password = userSubmittedEvent.Password;
            UserStatus = userSubmittedEvent.UserStatus;
            DateTimeCreated = userSubmittedEvent.DateTimeCreated;
        }
        
        internal void Apply(UserUpdatedEvent userUpdatedEvent)
        {
            EmailAddress = userUpdatedEvent.EmailAddress;
            UserStatus = userUpdatedEvent.UserStatus;
        }
        
        internal void Apply(UserDeletedEvent userDeletedEvent)
        {
        }
        
        internal void Apply(UserVerifiedEvent userVerifiedEvent)
        {
            UserStatus = userVerifiedEvent.UserStatus;
        }
    }
}