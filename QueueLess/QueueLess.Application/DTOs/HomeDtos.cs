using System;
using System.Collections.Generic;

namespace QueueLess.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string? Description { get; set; }
    }

    public class PopularServiceCardDto
    {
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string ActionText { get; set; } = "Book Now";
    }

    public class RecommendedBusinessDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Rating { get; set; }
        public bool IsOpen { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Address { get; set; } = string.Empty;
    }

    public class HomePageDataResponse
    {
        public string CurrentLocation { get; set; } = "Tanta City Center";
        public List<CategoryDto> Categories { get; set; } = new();
        public List<PopularServiceCardDto> PopularServices { get; set; } = new();
        public List<RecommendedBusinessDto> RecommendedServices { get; set; } = new();
    }
}
