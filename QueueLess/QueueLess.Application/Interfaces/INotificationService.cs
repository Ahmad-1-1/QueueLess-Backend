using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QueueLess.Application.DTOs;

namespace QueueLess.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync(NotificationContext context);
        Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
