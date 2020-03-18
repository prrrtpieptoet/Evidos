using System.Threading.Tasks;
using Evidos.EventSourcing.Domain.Core.Abstractions;

namespace Evidos.EventSourcing.EventHandling.Abstractions
{
    public interface IDomainEventHandler<in TEvent>
        where TEvent: IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }
}
