using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evidos.EventSourcing.Query.Entities;

namespace Evidos.EventSourcing.EventHandling.Abstractions
{
    public interface IUserReader
    {
        Task<User> GetByIdAsync(Guid id);

        Task InsertAsync(User user);

        Task UpdateAsync(User user);

        Task<IEnumerable<User>> QueryAsync(Func<User, bool> query);
    }
}
