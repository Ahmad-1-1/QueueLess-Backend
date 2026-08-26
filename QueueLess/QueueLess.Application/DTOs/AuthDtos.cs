using System;

namespace QueueLess.Application.DTOs
{
    public class RegisterRequest
    {
        /// <summary>Mobile number is required.</summary>
        public string MobileNumber { get; set; } = string.Empty;

        /// <summary>Email address is required.</summary>
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        /// <summary>Mobile number is required for login.</summary>
        public string MobileNumber { get; set; } = string.Empty;
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
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        /// <summary>Email address associated with the user account.</summary>
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        /// <summary>Email address associated with the user account.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>6-digit OTP code received via email.</summary>
        public string Otp { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        /// <summary>Email address associated with the user account.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>6-digit OTP code received via email.</summary>
        public string Otp { get; set; } = string.Empty;

        /// <summary>New password to set.</summary>
        public string NewPassword { get; set; } = string.Empty;
    }
}