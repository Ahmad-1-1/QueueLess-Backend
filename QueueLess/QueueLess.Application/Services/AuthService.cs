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

            // Check mobile number uniqueness (required)
            var existingByMobile = await _userRepository.GetByMobileNumberAsync(request.MobileNumber);
            if (existingByMobile != null)
                throw new InvalidOperationException("Mobile number is already registered.");

            // Check email uniqueness (optional — only if provided)
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingByEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingByEmail != null)
                    throw new InvalidOperationException("Email address is already registered.");
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                MobileNumber = request.MobileNumber,
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim().ToLower(),
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Role = Role.Customer,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new RegisterResponse
            {
                UserId = user.Id,
                MobileNumber = user.MobileNumber,
                Email = user.Email,
                FullName = user.FullName
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.", nameof(request.Password));

            bool hasMobile = !string.IsNullOrWhiteSpace(request.MobileNumber);
            bool hasEmail = !string.IsNullOrWhiteSpace(request.Email);

            if (!hasMobile && !hasEmail)
                throw new ArgumentException("A mobile number or email address is required to log in.");

            // Resolve user — mobile number takes priority
            User? user = null;
            if (hasMobile)
                user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber!);
            else
                user = await _userRepository.GetByEmailAsync(request.Email!);

            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                throw new UnauthorizedAccessException("Invalid credentials. Please check your mobile number, email, or password.");

            if (!user.IsActive)
                throw new InvalidOperationException("User account is inactive.");

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
