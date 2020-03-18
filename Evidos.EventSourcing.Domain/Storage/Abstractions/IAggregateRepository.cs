using System;
using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.Core.Abstractions;

namespace Evidos.EventSourcing.Domain.Storage.Abstractions
{
    public interface IAggregateRepository<TAggregate>
        where TAggregate: IAggregate
    {
        Task<TAggregate> GetByIdAsync(Guid id);

        Task SaveAsync(TAggregate aggregate);

        Task DeleteAsync(TAggregate aggregate);
    }
}
