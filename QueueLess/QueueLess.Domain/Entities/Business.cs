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

        // Geographic coordinates used for location-based recommendations.
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string? ImageUrl { get; set; }

        public double Rating { get; set; } = 4.5;

        // Used to rank businesses in Popular Services.
        public int PopularityScore { get; set; }

        public bool IsOpen { get; set; } = true;

        public Guid CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public BusinessCategory? Category { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}