using System;
using System.Threading.Tasks;
using QueueLess.Domain.Entities;

namespace QueueLess.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByMobileNumberAsync(string mobileNumber);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
