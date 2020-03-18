using System.Threading.Tasks;

namespace Evidos.EventSourcing.Domain.PubSub.Abstractions
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T publishedEvent);
    }
}
