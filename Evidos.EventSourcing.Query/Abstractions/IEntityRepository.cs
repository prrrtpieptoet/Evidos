using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evidos.EventSourcing.Query.Abstractions
{
    public interface IEntityRepository<T> where T: IEntity
    {
        Task InsertAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<IEnumerable<T>> QueryAsync(Func<T, bool> query);

        Task<T> GetByIdAsync(Guid id);
    }
}
