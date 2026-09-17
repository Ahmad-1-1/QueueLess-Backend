using System;

namespace QueueLess.Application.DTOs
{
    public class BusinessDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public double Rating { get; set; }
        public bool IsOpen { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class BusinessDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public double Rating { get; set; }
        public bool IsOpen { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class BusinessCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class BranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? BrandName { get; set; }
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceKm { get; set; }
        public double Rating { get; set; }
        public bool IsOpen { get; set; }
    }
}