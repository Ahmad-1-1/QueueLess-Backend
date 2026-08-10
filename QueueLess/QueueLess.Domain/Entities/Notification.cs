using System;

namespace QueueLess.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Navigation
        public Ticket? Ticket { get; set; }
        public User? User { get; set; }
    }
}
