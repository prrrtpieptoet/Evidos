using System;
using System.Threading.Tasks;

namespace Evidos.EventSourcing.EventHandling.Abstractions
{    
    public interface IUserWriter
    {
        Task CreateAsync(string emailAddress);

        Task UpdateAsync(Guid userId, string emailAddress);

        Task DeleteAsync(Guid userId);

        Task VerifyAsync(Guid userId);
    }
}
