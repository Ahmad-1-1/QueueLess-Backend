using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;

namespace QueueLess.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        [Authorize]
        [HttpPut("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!);
            await _authService.ChangePasswordAsync(userId, request);
            return Ok(new { message = "Password changed successfully." });
        }
        [HttpPost("password/forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _authService.ForgotPasswordAsync(request);
            return Ok(new { message = "If the account exists, a reset code has been sent to the registered email." });
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request);
            return Ok(new { message = "Password has been reset successfully." });
        }
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return NoContent();
        }
    }
}
