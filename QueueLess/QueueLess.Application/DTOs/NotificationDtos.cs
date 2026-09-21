using System;
using QueueLess.Domain.Enums;

namespace QueueLess.Application.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid? TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class NotificationContext
    {
        public Guid UserId { get; set; }
        public Guid? TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string? FcmToken { get; set; }
    }

    public class UpdateFcmTokenRequest
    {
        public string FcmToken { get; set; } = string.Empty;
    }
}
