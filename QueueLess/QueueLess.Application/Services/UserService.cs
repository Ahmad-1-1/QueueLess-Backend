using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace QueueLess.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileResponse> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            return MapToResponse(user);
        }

        public async Task<UserProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.", nameof(request.FullName));

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            user.FullName = request.FullName.Trim();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(user);
        }

        private static UserProfileResponse MapToResponse(User user) => new UserProfileResponse
        {
            UserId = user.Id,
            MobileNumber = user.MobileNumber,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString()
        };
    }
}
