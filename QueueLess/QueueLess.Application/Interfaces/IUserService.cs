using QueueLess.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLess.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponse> GetProfileAsync(Guid userId);
        Task<UserProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    }
}
