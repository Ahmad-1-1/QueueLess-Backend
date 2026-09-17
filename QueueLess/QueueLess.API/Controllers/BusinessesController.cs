using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;

namespace QueueLess.API.Controllers
{
    [ApiController]
    [Route("api/v1/businesses")]
    public class BusinessesController : ControllerBase
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly ITicketService _ticketService;

        public BusinessesController(IBusinessRepository businessRepository, ITicketService ticketService)
        {
            _businessRepository = businessRepository;
            _ticketService = ticketService;
        }

        /// <summary>
        /// Gets a list of active businesses with optional filtering
        /// by category, search term, or geographic location.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBusinesses(
            [FromQuery] string? category,
            [FromQuery] string? search,
            [FromQuery] double? latitude,
            [FromQuery] double? longitude)
        {
            var businesses = await _businessRepository
                .GetRecommendedBusinessesAsync(
                    latitude,
                    longitude,
                    category,
                    search);

            var result = businesses.Select(b => new BusinessDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Address = b.Address,
                ImageUrl = b.ImageUrl,
                Rating = b.Rating,
                IsOpen = b.IsOpen,

                CategoryId = b.CategoryId,
                CategoryName = b.Category?.Name ?? "General"
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Gets business details by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBusinessById(Guid id)
        {
            var business = await _businessRepository.GetByIdAsync(id);

            if (business == null)
            {
                return NotFound(new
                {
                    message = "Business not found."
                });
            }

            var result = new BusinessDetailsDto
            {
                Id = business.Id,
                Name = business.Name,
                Description = business.Description,
                Address = business.Address,
                ImageUrl = business.ImageUrl,
                Rating = business.Rating,
                IsOpen = business.IsOpen,

                CategoryId = business.CategoryId,
                CategoryName = business.Category?.Name ?? "General"
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets all business categories.
        /// </summary>
        [HttpGet("/api/v1/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _businessRepository.GetCategoriesAsync();

            var result = categories.Select(c => new BusinessCategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Gets available services for a specific business branch, including queue wait times and open counters.
        /// </summary>
        [HttpGet("{id:guid}/services")]
        public async Task<IActionResult> GetBusinessServices(Guid id)
        {
            var services = await _ticketService.GetServicesByBusinessIdAsync(id);
            return Ok(services);
        }

        /// <summary>
        /// Gets all branches of the selected business brand/chain, with distance relative to user location.
        /// Used to populate the Branch picker dropdown in the Reserve a Ticket screen.
        /// </summary>
        [HttpGet("{id:guid}/branches")]
        public async Task<IActionResult> GetBusinessBranches(
            Guid id,
            [FromQuery] double? latitude,
            [FromQuery] double? longitude)
        {
            var branches = await _businessRepository.GetBranchesAsync(id);

            var result = branches.Select(b =>
            {
                double? distanceKm = null;
                if (latitude.HasValue && longitude.HasValue && b.Latitude.HasValue && b.Longitude.HasValue)
                {
                    distanceKm = CalculateDistanceInKilometers(
                        latitude.Value, longitude.Value,
                        b.Latitude.Value, b.Longitude.Value);
                }

                return new BranchDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    BrandName = b.BrandName,
                    Address = b.Address,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    DistanceKm = distanceKm,
                    Rating = b.Rating,
                    IsOpen = b.IsOpen
                };
            })
            .OrderBy(b => b.DistanceKm ?? double.MaxValue)
            .ToList();

            return Ok(result);
        }

        private static double CalculateDistanceInKilometers(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371.0;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0);

            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            return Math.Round(earthRadiusKm * c, 1);
        }

        private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);
    }
}
