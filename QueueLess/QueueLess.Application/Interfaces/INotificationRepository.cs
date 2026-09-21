using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QueueLess.Domain.Entities;

namespace QueueLess.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<List<Notification>> GetByUserIdAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid id);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
