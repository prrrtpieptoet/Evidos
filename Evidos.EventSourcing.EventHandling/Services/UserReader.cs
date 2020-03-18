using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evidos.EventSourcing.EventHandling.Abstractions;
using Evidos.EventSourcing.Query.Abstractions;
using Evidos.EventSourcing.Query.Entities;

namespace Evidos.EventSourcing.EventHandling.Services
{
    public class UserReader : IUserReader
    {
        private readonly IEntityRepository<User> _userRepository;

        public UserReader(IEntityRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task InsertAsync(User user)
        {
            await _userRepository.InsertAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task<IEnumerable<User>> QueryAsync(Func<User, bool> query)
        {
            return await _userRepository.QueryAsync(query);
        }
    }
}
