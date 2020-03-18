using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.Core.Abstractions;
using Evidos.EventSourcing.Domain.Core.User;
using Evidos.EventSourcing.Domain.PubSub.Abstractions;
using Evidos.EventSourcing.Domain.Storage.Abstractions;
using Evidos.EventSourcing.EventHandling.Abstractions;

namespace Evidos.EventSourcing.EventHandling.Services
{
    public class UserWriter : IUserWriter
    {
        private readonly ISubscriber _subscriber;
        private readonly IAggregateRepository<User> _userRepository;
        private readonly IEnumerable<IDomainEventHandler<UserSubmittedEvent>> _userSubmittedEventHandlers;
        private readonly IEnumerable<IDomainEventHandler<UserUpdatedEvent>> _userUpdatedEventHandlers;
        private readonly IEnumerable<IDomainEventHandler<UserDeletedEvent>> _userDeleteEventHandlers;
        private readonly IEnumerable<IDomainEventHandler<UserVerifiedEvent>> _userVerifiedEventHandlers;

        public UserWriter(
            IAggregateRepository<User> useRepository,
            ISubscriber subscriber,
            IEnumerable<IDomainEventHandler<UserSubmittedEvent>> userSubmittedEventHandlers,
            IEnumerable<IDomainEventHandler<UserUpdatedEvent>> userUpdatedEventHandlers,
            IEnumerable<IDomainEventHandler<UserDeletedEvent>> userDeletedEventHandlerss,
            IEnumerable<IDomainEventHandler<UserVerifiedEvent>> userVerifiedEventHandlers)
        {
            _subscriber = subscriber;
            _userRepository = useRepository;
            _userSubmittedEventHandlers = userSubmittedEventHandlers;
            _userUpdatedEventHandlers = userUpdatedEventHandlers;
            _userDeleteEventHandlers = userDeletedEventHandlerss;
            _userVerifiedEventHandlers = userVerifiedEventHandlers;
        }

        public async Task CreateAsync(string emailAddress)
        {
            var user = new User(Guid.NewGuid(), emailAddress);

            _subscriber.Subscribe<UserSubmittedEvent>(async domainEvent => await HandleAsync(_userSubmittedEventHandlers, domainEvent));
            await _userRepository.SaveAsync(user);
        }

        public async Task UpdateAsync(Guid userId, string emailAddress)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            _subscriber.Subscribe<UserUpdatedEvent>(async domainEvent => await HandleAsync(_userUpdatedEventHandlers, domainEvent));
            user.UpdateEmailAddress(userId, emailAddress);
            await _userRepository.SaveAsync(user);
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            _subscriber.Subscribe<UserDeletedEvent>(async domainEvent => await HandleAsync(_userDeleteEventHandlers, domainEvent));
            user.Delete(userId);
            await _userRepository.DeleteAsync(user);
        }

        public async Task VerifyAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            _subscriber.Subscribe<UserVerifiedEvent>(async domainEvent => await HandleAsync(_userVerifiedEventHandlers, domainEvent));
            user.VerifyEmailAddress(userId);
            await _userRepository.SaveAsync(user);
        }

        public async Task HandleAsync<T>(IEnumerable<IDomainEventHandler<T>> handlers, T domainEvent)
            where T : IDomainEvent
        {
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(domainEvent);
            }
        }
    }
}
