using System;

namespace QueueLess.Domain.Entities
{
    public class Staff
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? ServiceId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public User? User { get; set; }
        public Service? Service { get; set; }
    }
}
