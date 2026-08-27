using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QueueLess.Application.Interfaces;

namespace QueueLess.API.Middleware
{
    /// <summary>
    /// Middleware that rejects requests whose JWT token has been blacklisted (e.g. after logout).
    /// Must be placed AFTER UseAuthentication() so that User.Claims are already populated.
    /// </summary>
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklist)
        {
            // Only check if the request has a valid authenticated user
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti) && blacklist.IsBlacklisted(jti))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        statusCode = 401,
                        message = "Token has been invalidated. Please log in again."
                    });
                    return;
                }
            }

            await _next(context);
        }
    }
}
