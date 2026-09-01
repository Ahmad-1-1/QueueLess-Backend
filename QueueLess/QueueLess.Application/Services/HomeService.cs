using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;

namespace QueueLess.Application.Services
{
    public class HomeService : IHomeService
    {
        private const double NearbyRadiusKm = 5.0;
        private const double RecommendedRadiusKm = 20.0;
        private const int PopularServicesCount = 5;

        private readonly IBusinessRepository _businessRepository;
        private readonly IPictureResolver _pictureResolver;

        public HomeService(
            IBusinessRepository businessRepository,
            IPictureResolver pictureResolver)
        {
            _businessRepository = businessRepository;
            _pictureResolver = pictureResolver;
        }

        public async Task<HomePageDataResponse> GetHomePageDataAsync(
            double? latitude = null,
            double? longitude = null)
        {
            var dbCategories =
                await _businessRepository.GetCategoriesAsync();

            var popularBusinesses =
                await _businessRepository.GetPopularBusinessesAsync(
                    PopularServicesCount);

            var recommendedBusinesses =
                await _businessRepository.GetRecommendedBusinessesAsync(
                    latitude,
                    longitude);

            // ============================================================
            // Categories
            // ============================================================

            var categories = dbCategories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconUrl = _pictureResolver.ResolveCategoryIcon(
                        c.IconUrl),
                    Description = c.Description
                })
                .ToList();

            // ============================================================
            // Popular Services
            // ============================================================

            var popularServices = popularBusinesses
                .Select(b => new PopularServiceCardDto
                {
                    BusinessId = b.Id,
                    CategoryId = b.CategoryId,

                    Title = b.Name,

                    Subtitle = b.Category?.Description
                               ?? b.Category?.Name
                               ?? "General Service",

                    ImageUrl =
                        _pictureResolver.ResolvePopularServiceImage(
                            b.ImageUrl),

                    Rating = b.Rating,

                    ActionText = "Book Now"
                })
                .ToList();

            // ============================================================
            // Recommended Businesses
            // ============================================================

            var recommendedServices = recommendedBusinesses
                .Select(b =>
                {
                    double? distanceKm = null;

                    if (latitude.HasValue &&
                        longitude.HasValue &&
                        b.Latitude.HasValue &&
                        b.Longitude.HasValue)
                    {
                        distanceKm = CalculateDistanceKm(
                            latitude.Value,
                            longitude.Value,
                            b.Latitude.Value,
                            b.Longitude.Value);
                    }

                    return new
                    {
                        Business = b,
                        DistanceKm = distanceKm
                    };
                })
                .Where(x =>
                    !x.DistanceKm.HasValue ||
                    x.DistanceKm.Value <= RecommendedRadiusKm)
                .OrderBy(x =>
                    x.DistanceKm.HasValue
                        ? x.DistanceKm.Value
                        : double.MaxValue)
                .ThenByDescending(x => x.Business.Rating)
                .ThenByDescending(x => x.Business.PopularityScore)
                .Select(x => new RecommendedBusinessDto
                {
                    Id = x.Business.Id,

                    Name = x.Business.Name,

                    CategoryName =
                        x.Business.Category?.Name
                        ?? "General",

                    Address = x.Business.Address,

                    Description = x.Business.Description,

                    Rating = x.Business.Rating,

                    IsOpen = x.Business.IsOpen,

                    Tag = GetRecommendationTag(
                        x.DistanceKm,
                        x.Business.PopularityScore),

                    DistanceKm = x.DistanceKm.HasValue
                        ? Math.Round(x.DistanceKm.Value, 2)
                        : null,

                    ImageUrl =
                        _pictureResolver.ResolveBusinessImage(
                            x.Business.ImageUrl)
                })
                .ToList();

            // ============================================================
            // User Location
            // ============================================================

            UserLocationDto? userLocation = null;

            if (latitude.HasValue && longitude.HasValue)
            {
                userLocation = new UserLocationDto
                {
                    Latitude = latitude.Value,
                    Longitude = longitude.Value
                };
            }

            // ============================================================
            // Final Home Response
            // ============================================================

            return new HomePageDataResponse
            {
                UserLocation = userLocation,

                Categories = categories,

                PopularServices = popularServices,

                RecommendedServices = recommendedServices
            };
        }

        // ================================================================
        // Recommendation Tag
        // ================================================================

        private static string GetRecommendationTag(
            double? distanceKm,
            int popularityScore)
        {
            if (distanceKm.HasValue &&
                distanceKm.Value <= NearbyRadiusKm)
            {
                return "Nearby";
            }

            if (popularityScore > 0)
            {
                return "Popular";
            }

            return string.Empty;
        }

        // ================================================================
        // Haversine Distance
        // ================================================================

        private static double CalculateDistanceKm(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
        {
            const double earthRadiusKm = 6371.0;

            var latitudeDifference =
                DegreesToRadians(latitude2 - latitude1);

            var longitudeDifference =
                DegreesToRadians(longitude2 - longitude1);

            var latitude1Radians =
                DegreesToRadians(latitude1);

            var latitude2Radians =
                DegreesToRadians(latitude2);

            var a =
                Math.Sin(latitudeDifference / 2) *
                Math.Sin(latitudeDifference / 2)
                +
                Math.Cos(latitude1Radians) *
                Math.Cos(latitude2Radians) *
                Math.Sin(longitudeDifference / 2) *
                Math.Sin(longitudeDifference / 2);

            var c =
                2 * Math.Atan2(
                    Math.Sqrt(a),
                    Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}