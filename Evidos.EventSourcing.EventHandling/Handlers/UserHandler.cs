using System;
using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.Core.User;
using Evidos.EventSourcing.EventHandling.Handlers.Abstractions;
using Evidos.EventSourcing.Query.Abstractions;
using User = Evidos.EventSourcing.Query.Entities.User;

namespace Evidos.EventSourcing.EventHandling.Handlers
{
    public class UserHandler :
        IDomainEventHandler<UserSubmittedEvent>,
        IDomainEventHandler<UserUpdatedEvent>,
        IDomainEventHandler<UserDeletedEvent>,
        IDomainEventHandler<UserVerifiedEvent>
    {
        private readonly IEntityRepository<User> _userRepository;

        public UserHandler(IEntityRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserSubmittedEvent domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            await _userRepository.InsertAsync(new User
            {
                Id = domainEvent.AggregateId,
                EmailAddress = domainEvent.EmailAddress,
                Password = domainEvent.Password,
                Status = domainEvent.UserStatus.ToString(),
                DateTimeCreated = domainEvent.DateTimeCreated
            });
        }

        public async Task HandleAsync(UserUpdatedEvent domainEvent)
        {
            var user = await _userRepository.GetByIdAsync(domainEvent.AggregateId);

            user.EmailAddress = domainEvent.EmailAddress;
            user.Status = domainEvent.UserStatus.ToString();

            await _userRepository.UpdateAsync(user);
        }

        public async Task HandleAsync(UserDeletedEvent domainEvent)
        {
            var user = await _userRepository.GetByIdAsync(domainEvent.AggregateId);

            await _userRepository.DeleteAsync(user);
        }

        public async Task HandleAsync(UserVerifiedEvent domainEvent)
        {
            var user = await _userRepository.GetByIdAsync(domainEvent.AggregateId);

            user.Status = domainEvent.UserStatus.ToString();

            await _userRepository.UpdateAsync(user);
        }
    }
}
