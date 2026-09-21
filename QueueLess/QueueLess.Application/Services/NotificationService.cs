using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;

namespace QueueLess.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEnumerable<INotificationStrategy> _strategies;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IEnumerable<INotificationStrategy> strategies,
            INotificationRepository notificationRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<NotificationService> logger)
        {
            _strategies = strategies;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task NotifyAsync(NotificationContext context)
        {
            // If FcmToken was not supplied in context, attempt to lookup from User profile
            if (string.IsNullOrWhiteSpace(context.FcmToken) && context.UserId != Guid.Empty)
            {
                var user = await _userRepository.GetByIdAsync(context.UserId);
                if (user != null)
                {
                    context.FcmToken = user.FcmToken;
                }
            }

            // Dispatch through all registered strategies (InApp, Push, etc.)
            foreach (var strategy in _strategies)
            {
                try
                {
                    await strategy.SendAsync(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing notification strategy {StrategyName} for user {UserId}",
                        strategy.GetType().Name, context.UserId);
                }
            }
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                TicketId = n.TicketId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                SentAt = n.SentAt
            }).ToList();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId, userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
