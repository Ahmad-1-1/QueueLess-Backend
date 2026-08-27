using System;
using System.Collections.Generic;

namespace QueueLess.Domain.Entities
{
    public class Business
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty; // e.g. "Downtown", "Central District", "Riverside"
        public string? ImageUrl { get; set; }
        public double Rating { get; set; } = 4.5;
        public string? Tag { get; set; } // e.g. "Popular", "Nearby"
        public bool IsOpen { get; set; } = true;
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public BusinessCategory? Category { get; set; }
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
