using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Evidos.EventSourcing.Query.Abstractions;
using Evidos.EventSourcing.Query.Exceptions;

namespace Evidos.EventSourcing.Query.InMemory
{
    public class EntityStore<T> : IEntityRepository<T> where T : IEntity
    {
        private readonly Lazy<IList<T>> _entityStore = new Lazy<IList<T>>(() => new List<T>());
        private readonly ReaderWriterLockSlim _entityStoreLock = new ReaderWriterLockSlim();

        public async Task<IEnumerable<T>> QueryAsync(Func<T, bool> query)
        {
            ReadLockEntityStore();

            try
            {
                return await Task.FromResult(_entityStore.Value.Where(query));
            }
            finally
            {
                _entityStoreLock.ExitReadLock();
            }
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            ReadLockEntityStore();

            try
            {
                return await Task.FromResult(_entityStore.Value.Single(ety => ety.Id == id));
            }
            finally
            {
                _entityStoreLock.ExitReadLock();
            }
        }

        public async Task InsertAsync(T entity)
        {
            WriteLockEntityStore();

            try
            {
                if (_entityStore.Value.Any(ety => ety.Id == entity.Id))
                    throw new EntityRepositoryException($"Error adding entity: id {entity.Id} already exists.");

                _entityStore.Value.Add(entity);
                await Task.CompletedTask;
            }
            finally
            {
                _entityStoreLock.ExitWriteLock();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            WriteLockEntityStore();

            try
            {
                var storedEntity = _entityStore.Value.SingleOrDefault(ety => ety.Id == entity.Id);

                if (storedEntity == null)
                    throw new EntityRepositoryException($"Error updating entity: id {entity.Id} does not exist.");


                if (!_entityStore.Value.Remove(storedEntity))
                    throw new EntityRepositoryException($"Error updating entity: id {entity.Id} could not be replaced.");

                _entityStore.Value.Add(entity);
                await Task.CompletedTask;
            }
            finally
            {
                _entityStoreLock.ExitWriteLock();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            WriteLockEntityStore();

            try
            {
                var storedEntity = _entityStore.Value.SingleOrDefault(ety => ety.Id == entity.Id);

                if (storedEntity == null)
                    throw new EntityRepositoryException($"Error deleting entity: id {entity.Id} does not exist.");


                if (!_entityStore.Value.Remove(storedEntity))
                    throw new EntityRepositoryException($"Error deleting entity: id {entity.Id} could not be removed.");
                await Task.CompletedTask;
            }
            finally
            {
                _entityStoreLock.ExitWriteLock();
            }
        }

        private void ReadLockEntityStore()
        {
            if (!_entityStoreLock.TryEnterReadLock(TimeSpan.FromSeconds(3)))
                throw new EntityRepositoryException(
                    $"Error executing query: Cannot obtain read lock for entity store.");

        }
        
        private void WriteLockEntityStore()
        {
            if (!_entityStoreLock.TryEnterWriteLock(TimeSpan.FromSeconds(3)))
                throw new EntityRepositoryException(
                    $"Error executing data manipulation: Cannot obtain write lock for entity store.");

        }
    }
}
