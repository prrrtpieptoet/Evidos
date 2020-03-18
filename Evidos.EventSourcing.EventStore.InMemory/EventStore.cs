using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.Core.Abstractions;
using Evidos.EventSourcing.Domain.Storage.EventStore;
using Evidos.EventSourcing.Domain.Storage.EventStore.Abstractions;
using Evidos.EventSourcing.Domain.Storage.EventStore.Exceptions;

namespace Evidos.EventSourcing.EventStore.InMemory
{
    public class EventStore : IEventStore
    {
        private readonly Lazy<IList<Event>> _eventStore = new Lazy<IList<Event>>(() => new List<Event>());
        private readonly ReaderWriterLockSlim _eventStoreLock = new ReaderWriterLockSlim();

        public async Task<IEnumerable<Event>> ReadEventsAsync(Guid aggregateId)
        {
            if (_eventStoreLock.TryEnterReadLock(TimeSpan.FromSeconds(3)))
            {
                try
                {
                    return await Task.FromResult(_eventStore.Value
                        .Where(evt => evt.DomainEvent.AggregateId == aggregateId)
                        .OrderBy(evt => evt.EventNumber));
                }
                finally
                {
                    _eventStoreLock.ExitReadLock();
                }
            }

            throw new EventStoreException(
                $"Error retrieving events for aggregate with id {aggregateId}:" +
                $" Cannot obtain read lock for event store.");
        }

        public async Task AppendEventAsync(IDomainEvent domainEvent)
        {
            if (domainEvent != null)
            {
                if (_eventStoreLock.TryEnterWriteLock(TimeSpan.FromSeconds(3)))
                {
                    try
                    {
                        _eventStore.Value.Add(new Event(domainEvent, 1 + GetHighestEventNumber(domainEvent.AggregateId)));
                    }
                    finally
                    {
                        _eventStoreLock.ExitWriteLock();
                    }
                }
                else
                {
                    throw new EventStoreException(
                        $"Error appending event with id {domainEvent.EventId} for aggregate with id {domainEvent.AggregateId}:" +
                        $" Cannot obtain write lock for event store.");
                }
            }
            
            await Task.CompletedTask;
        }

        public async Task AppendEventsAsync(LinkedList<IDomainEvent> domainEvents)
        {
            if (domainEvents != null)
            {
                if (_eventStoreLock.TryEnterWriteLock(TimeSpan.FromSeconds(3)))
                {
                    try
                    {
                        long? highestEventNumber = null;

                        var events = domainEvents
                            .Select((domainEvent, i) =>
                            {
                                if (highestEventNumber == null)
                                    highestEventNumber = GetHighestEventNumber(domainEvent.AggregateId);

                                return new Event(domainEvent, highestEventNumber.Value + (i + 1));
                            }).ToList();

                        events.ForEach(evt => _eventStore.Value.Add(evt));
                    }
                    finally
                    {
                        _eventStoreLock.ExitWriteLock();
                    }
                }
                else
                {
                    throw new EventStoreException(
                        $"Error appending events: Cannot obtain write lock for event store.");
                }
            }

            await Task.CompletedTask;
        }

        public async Task DeleteEventsAsync(Guid aggregateId)
        {
            if (_eventStoreLock.TryEnterWriteLock(TimeSpan.FromSeconds(3)))
            {
                try
                {
                    _eventStore.Value
                        .Where(evt => evt.DomainEvent.AggregateId == aggregateId)
                        .ToList()
                        .ForEach(evt => _eventStore.Value.Remove(evt));
                }
                finally
                {
                    _eventStoreLock.ExitWriteLock();
                }
            }
            else
            {
                throw new EventStoreException(
                    $"Error removing events for aggregate with id {aggregateId}:" +
                    $" Cannot obtain write lock for event store.");
            }

            await Task.CompletedTask;
        }

        private long GetHighestEventNumber(Guid aggregateId)
        {
            var aggregateEvents = _eventStore.Value
                .Where(evt => evt.DomainEvent.AggregateId == aggregateId).ToList();

            return aggregateEvents.Any()
                ? aggregateEvents.Max(evt => evt.EventNumber) : -1;
        }
    }
}
