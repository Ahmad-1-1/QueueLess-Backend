using System;
using System.Collections.Generic;

namespace QueueLess.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? AssignedStaffId { get; set; }
        public bool IsActive { get; set; } = true;
        public int AvgServiceTimeMinutes { get; set; }

        // Navigation
        public Business? Business { get; set; }
        public Staff? AssignedStaff { get; set; }
        public ICollection<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
