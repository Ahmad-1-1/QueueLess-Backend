using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;

namespace QueueLess.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly IBusinessRepository _businessRepository;

        public HomeService(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }

        public async Task<HomePageDataResponse> GetHomePageDataAsync(string? category = null, string? search = null, string? location = null)
        {
            var dbCategories = await _businessRepository.GetCategoriesAsync();
            var dbBusinesses = await _businessRepository.GetRecommendedBusinessesAsync(category, search, location);

            // Default categories matching Figma UI if DB has none
            var categoryList = new List<CategoryDto>();
            if (dbCategories.Any())
            {
                categoryList = dbCategories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconUrl = c.IconUrl,
                    Description = c.Description
                }).ToList();
            }
            else
            {
                categoryList = new List<CategoryDto>
                {
                    new CategoryDto { Id = Guid.NewGuid(), Name = "All" },
                    new CategoryDto { Id = Guid.NewGuid(), Name = "Bank", Description = "Financial Transactions" },
                    new CategoryDto { Id = Guid.NewGuid(), Name = "Hospital", Description = "Medical & Consultations" },
                    new CategoryDto { Id = Guid.NewGuid(), Name = "Clinic", Description = "Check-ups & Vaccinations" }
                };
            }

            // Popular service cards matching Figma UI banner
            var popularServices = new List<PopularServiceCardDto>
            {
                new PopularServiceCardDto
                {
                    CategoryId = categoryList.FirstOrDefault(c => c.Name == "Hospital")?.Id ?? Guid.Empty,
                    Title = "Hospital",
                    Subtitle = "Medical & Consultations",
                    ImageUrl = "https://images.unsplash.com/photo-1587351021759-3e566b6af7cc?w=600",
                    ActionText = "Book Now"
                },
                new PopularServiceCardDto
                {
                    CategoryId = categoryList.FirstOrDefault(c => c.Name == "Bank")?.Id ?? Guid.Empty,
                    Title = "Bank",
                    Subtitle = "Financial Transactions",
                    ImageUrl = "https://images.unsplash.com/photo-1541354329998-f4d9a9f9297f?w=600",
                    ActionText = "Book Now"
                }
            };

            // Recommended services matching Figma UI cards
            var recommendedServices = new List<RecommendedBusinessDto>();

            if (dbBusinesses.Any())
            {
                recommendedServices = dbBusinesses.Select(b => new RecommendedBusinessDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    CategoryName = b.Category?.Name ?? "General",
                    Location = string.IsNullOrWhiteSpace(b.Location) ? "Downtown" : b.Location,
                    Description = b.Description,
                    Rating = b.Rating,
                    IsOpen = b.IsOpen,
                    Tag = string.IsNullOrWhiteSpace(b.Tag) ? "Popular" : b.Tag,
                    ImageUrl = b.ImageUrl ?? "https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?w=600",
                    Address = b.Address
                }).ToList();
            }
            else
            {
                // Figma UI sample fallback items
                recommendedServices = new List<RecommendedBusinessDto>
                {
                    new RecommendedBusinessDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "City General Hospital",
                        CategoryName = "Hospital",
                        Location = "Downtown",
                        Description = "Emergency care, specialist consultation",
                        Rating = 4.6,
                        IsOpen = true,
                        Tag = "Popular",
                        ImageUrl = "https://images.unsplash.com/photo-1587351021759-3e566b6af7cc?w=600",
                        Address = "123 Main St, Downtown"
                    },
                    new RecommendedBusinessDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Metro Bank – CBD Branch",
                        CategoryName = "Bank",
                        Location = "Central District",
                        Description = "Accounts, loans, foreign exchange",
                        Rating = 4.5,
                        IsOpen = true,
                        Tag = "Popular",
                        ImageUrl = "https://images.unsplash.com/photo-1541354329998-f4d9a9f9297f?w=600",
                        Address = "456 Central Ave, CBD"
                    },
                    new RecommendedBusinessDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "National Health Clinic",
                        CategoryName = "Clinic",
                        Location = "Riverside",
                        Description = "Check-ups, vaccinations",
                        Rating = 4.8,
                        IsOpen = true,
                        Tag = "Nearby",
                        ImageUrl = "https://images.unsplash.com/photo-1629909613654-28e377c37b09?w=600",
                        Address = "789 River Rd, Riverside"
                    }
                };
            }

            return new HomePageDataResponse
            {
                CurrentLocation = string.IsNullOrWhiteSpace(location) ? "Tanta City Center" : location,
                Categories = categoryList,
                PopularServices = popularServices,
                RecommendedServices = recommendedServices
            };
        }
    }
}
