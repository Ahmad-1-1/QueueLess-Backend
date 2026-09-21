using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueLess.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QueueLess.API.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException("Invalid token.")
            );

        // GET: api/v1/notifications
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var notifications =
                await _notificationService.GetUserNotificationsAsync(CurrentUserId);

            return Ok(notifications);
        }

        // GET: api/v1/notifications/unread-count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count =
                await _notificationService.GetUnreadCountAsync(CurrentUserId);

            return Ok(new
            {
                unreadCount = count
            });
        }

        // PATCH: api/v1/notifications/{notificationId}/read
        [HttpPatch("{notificationId:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            await _notificationService.MarkAsReadAsync(
                notificationId,
                CurrentUserId);

            return Ok(new
            {
                message = "Notification marked as read."
            });
        }

        // PATCH: api/v1/notifications/read-all
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(CurrentUserId);

            return Ok(new
            {
                message = "All notifications marked as read."
            });
        }
    }
}