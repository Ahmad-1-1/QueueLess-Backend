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

        public BusinessesController(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
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
    }
}