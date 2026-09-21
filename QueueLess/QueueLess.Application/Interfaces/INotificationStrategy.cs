using System.Threading.Tasks;
using QueueLess.Application.DTOs;

namespace QueueLess.Application.Interfaces
{
    /// <summary>
    /// Strategy interface for dispatching notifications across multiple channels
    /// (e.g. In-App database storage, Firebase Cloud Messaging push, SMS, etc.).
    /// </summary>
    public interface INotificationStrategy
    {
        Task SendAsync(NotificationContext context);
    }
}
