using System;
using System.Threading.Tasks;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;
using QueueLess.Domain.Enums;

namespace QueueLess.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                throw new ArgumentException("Mobile number is required.", nameof(request.MobileNumber));
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.", nameof(request.Password));
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.", nameof(request.FullName));

            // Check if mobile number is already registered
            var existingUser = await _userRepository.GetByMobileNumberAsync(request.MobileNumber);
            if (existingUser != null)
            {
                // We'll throw a distinct exception or standard ApplicationException that will map to 409 Conflict.
                throw new InvalidOperationException("Mobile number is already registered.");
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                MobileNumber = request.MobileNumber,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Role = Role.Customer, // Default role for public registration
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new RegisterResponse
            {
                UserId = user.Id,
                MobileNumber = user.MobileNumber,
                FullName = user.FullName
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                throw new ArgumentException("Mobile number is required.", nameof(request.MobileNumber));
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.", nameof(request.Password));

            var user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber);
            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                // Throw UnauthorizedAccessException, which our middleware will map to 401 Unauthorized
                throw new UnauthorizedAccessException("Invalid mobile number or password.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("User account is inactive.");
            }

            var token = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                Role = user.Role.ToString()
            };
        }
    }
}
