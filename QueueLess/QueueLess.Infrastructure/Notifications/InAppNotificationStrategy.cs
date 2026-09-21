using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;

namespace QueueLess.Infrastructure.Notifications
{
    public class InAppNotificationStrategy : INotificationStrategy
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InAppNotificationStrategy> _logger;

        public InAppNotificationStrategy(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            ILogger<InAppNotificationStrategy> logger)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SendAsync(NotificationContext context)
        {
            try
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = context.UserId,
                    TicketId = context.TicketId,
                    Title = context.Title,
                    Message = context.Message,
                    Type = context.Type,
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _notificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("In-App notification [{Title}] persisted for user {UserId}", context.Title, context.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist In-App notification for user {UserId}", context.UserId);
            }
        }
    }
}
