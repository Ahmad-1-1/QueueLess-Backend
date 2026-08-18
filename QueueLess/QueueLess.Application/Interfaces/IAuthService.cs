using System.Threading.Tasks;
using QueueLess.Application.DTOs;

namespace QueueLess.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task VerifyOtpAsync(VerifyOtpRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}
