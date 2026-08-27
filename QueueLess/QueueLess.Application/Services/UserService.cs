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
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserProfileResponse> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            return MapToResponse(user);
        }


        public async Task<UserProfileResponse> UpdatePhoneAsync(Guid userId, UpdatePhoneRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                throw new ArgumentException("Mobile number is required.", nameof(request.MobileNumber));

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            var newMobile = request.MobileNumber.Trim();
            if (!string.Equals(user.MobileNumber, newMobile, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userRepository.GetByMobileNumberAsync(newMobile);
                if (existing != null && existing.Id != user.Id)
                {
                    throw new InvalidOperationException("Mobile number is already registered by another account.");
                }
                user.MobileNumber = newMobile;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToResponse(user);
        }

        public async Task<UserProfileResponse> UpdateEmailAsync(Guid userId, UpdateEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email address is required.", nameof(request.Email));

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            var newEmail = request.Email.Trim().ToLower();
            if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userRepository.GetByEmailAsync(newEmail);
                if (existing != null && existing.Id != user.Id)
                {
                    throw new InvalidOperationException("Email address is already registered by another account.");
                }
                user.Email = newEmail;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToResponse(user);
        }

        public async Task<UserProfileResponse> UpdateNameAsync(Guid userId, UpdateNameRequest request)
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
