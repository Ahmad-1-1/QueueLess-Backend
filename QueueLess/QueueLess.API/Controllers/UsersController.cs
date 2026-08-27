using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QueueLess.API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException("Invalid token."));

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var result = await _userService.GetProfileAsync(CurrentUserId);
            return Ok(result);
        }

        [HttpPut("me/phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] UpdatePhoneRequest request)
        {
            var result = await _userService.UpdatePhoneAsync(CurrentUserId, request);
            return Ok(result);
        }

        [HttpPut("me/email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
        {
            var result = await _userService.UpdateEmailAsync(CurrentUserId, request);
            return Ok(result);
        }

        [HttpPut("me/name")]
        public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
        {
            var result = await _userService.UpdateNameAsync(CurrentUserId, request);
            return Ok(result);
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await _authService.ChangePasswordAsync(CurrentUserId, request);
            return Ok(new { message = "Password changed successfully." });
        }
    }
}
