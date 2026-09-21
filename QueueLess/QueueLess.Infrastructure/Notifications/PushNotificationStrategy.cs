using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;

namespace QueueLess.Infrastructure.Notifications
{
    /// <summary>
    /// Push notification strategy (FCM / Firebase Cloud Messaging).
    /// Dispatches push notifications when user has a registered FcmToken.
    /// Phase 2: Fill in FirebaseAdmin SDK calls when credentials JSON is provided.
    /// </summary>
    public class PushNotificationStrategy : INotificationStrategy
    {
        private readonly ILogger<PushNotificationStrategy> _logger;

        public PushNotificationStrategy(ILogger<PushNotificationStrategy> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(NotificationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.FcmToken))
            {
                _logger.LogDebug("User {UserId} has no FCM token registered. Skipping push notification.", context.UserId);
                return Task.CompletedTask;
            }

            // Phase 2: FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendAsync(...)
            _logger.LogInformation("Simulated Push Notification to FCM Token [{Token}]: {Title} - {Message}",
                context.FcmToken, context.Title, context.Message);

            return Task.CompletedTask;
        }
    }
}
