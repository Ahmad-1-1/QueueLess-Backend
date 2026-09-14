using System;
using QueueLess.Domain.Enums;

namespace QueueLess.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid CustomerId { get; set; }
        public int QueueNumber { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string QrCodeData { get; set; } = string.Empty;
        public bool NotifyByPush { get; set; } = true;
        public bool NotifyByEmail { get; set; } = false;
        public TicketStatus Status { get; set; } = TicketStatus.Waiting;
        public int PositionSnapshot { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ServedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public Service? Service { get; set; }
        public User? Customer { get; set; }
    }
}
