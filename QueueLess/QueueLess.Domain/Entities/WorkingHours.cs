using System;

namespace QueueLess.Domain.Entities
{
    public class WorkingHours
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, etc.
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        // Navigation
        public Service? Service { get; set; }
    }
}
