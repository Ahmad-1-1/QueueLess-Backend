using QueueLess.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace QueueLess.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponse> GetProfileAsync(Guid userId);
        Task<UserProfileResponse> UpdatePhoneAsync(Guid userId, UpdatePhoneRequest request);
        Task<UserProfileResponse> UpdateEmailAsync(Guid userId, UpdateEmailRequest request);
        Task<UserProfileResponse> UpdateNameAsync(Guid userId, UpdateNameRequest request);
    }
}

