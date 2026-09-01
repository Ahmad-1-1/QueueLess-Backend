using System;
using System.Collections.Generic;

namespace QueueLess.Application.DTOs
{
    public class UserLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string? Description { get; set; }
    }

    public class PopularServiceCardDto
    {
        public Guid BusinessId { get; set; }
        public Guid CategoryId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public double Rating { get; set; }

        public string ActionText { get; set; } = "Book Now";
    }

    public class RecommendedBusinessDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public double Rating { get; set; }

        public bool IsOpen { get; set; }

        public string Tag { get; set; } = string.Empty;

        public double? DistanceKm { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class HomePageDataResponse
    {
        public UserLocationDto? UserLocation { get; set; }

        public List<CategoryDto> Categories { get; set; } = new();

        public List<PopularServiceCardDto> PopularServices { get; set; } = new();

        public List<RecommendedBusinessDto> RecommendedServices { get; set; } = new();
    }
}