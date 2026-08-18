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
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IOtpRepository otpRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _otpRepository = otpRepository;
            _emailService = emailService;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                throw new ArgumentException("Mobile number is required.", nameof(request.MobileNumber));
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.", nameof(request.Password));
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.", nameof(request.FullName));

            var mobile = request.MobileNumber.Trim();
            var existingByMobile = await _userRepository.GetByMobileNumberAsync(mobile);
            if (existingByMobile != null)
                throw new InvalidOperationException("Mobile number is already registered.");

            string? email = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                email = request.Email.Trim().ToLower();
                var existingByEmail = await _userRepository.GetByEmailAsync(email);
                if (existingByEmail != null)
                    throw new InvalidOperationException("Email address is already registered.");
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                MobileNumber = mobile,
                Email = email,
                PasswordHash = passwordHash,
                FullName = request.FullName.Trim(),
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

            var hasMobile = !string.IsNullOrWhiteSpace(request.MobileNumber);
            var hasEmail = !string.IsNullOrWhiteSpace(request.Email);

            if (!hasMobile && !hasEmail)
                throw new ArgumentException("A mobile number or email address is required to log in.");

            User? user = null;
            if (hasMobile)
                user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber!.Trim());

            if (user == null && hasEmail)
                user = await _userRepository.GetByEmailAsync(request.Email!.Trim().ToLower());

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

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                throw new ArgumentException("Old and new passwords are required.");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.OldPassword))
                throw new UnauthorizedAccessException("Old password is incorrect.");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AccountIdentifier))
                throw new ArgumentException("Mobile number or email address is required.");

            var identifier = request.AccountIdentifier.Trim();
            var user = await _userRepository.GetByMobileNumberAsync(identifier)
                       ?? await _userRepository.GetByEmailAsync(identifier.ToLower());

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
                return;

            var otpCode = Random.Shared.Next(100000, 999999).ToString();

            var otp = new OtpRequest
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OtpCode = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _otpRepository.AddAsync(otp);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(user.Email, user.FullName, otpCode);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AccountIdentifier) ||
                string.IsNullOrWhiteSpace(request.Otp) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
                throw new ArgumentException("All fields are required.");

            var identifier = request.AccountIdentifier.Trim();
            var user = await _userRepository.GetByMobileNumberAsync(identifier)
                       ?? await _userRepository.GetByEmailAsync(identifier.ToLower())
                       ?? throw new UnauthorizedAccessException("Invalid request.");

            var otp = await _otpRepository.GetLatestValidOtpAsync(user.Id, request.Otp.Trim())
                ?? throw new UnauthorizedAccessException("Invalid or expired OTP code.");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            otp.IsUsed = true;

            await _userRepository.UpdateAsync(user);
            await _otpRepository.UpdateAsync(otp);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
