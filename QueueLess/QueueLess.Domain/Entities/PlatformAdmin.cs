using System;

namespace QueueLess.Domain.Entities
{
    public class PlatformAdmin
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
