using System;

namespace QueueLess.Application.DTOs
{
    public class RegisterRequest
    {
        /// <summary>Mobile number is required.</summary>
        public string MobileNumber { get; set; } = string.Empty;

        /// <summary>Email is optional but must be unique if provided.</summary>
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        /// <summary>
        /// Login using MobileNumber OR Email address.
        /// </summary>
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class UserProfileResponse
    {
        public Guid UserId { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        /// <summary>Mobile number or email address.</summary>
        public string AccountIdentifier { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        /// <summary>Mobile number or email address.</summary>
        public string AccountIdentifier { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}