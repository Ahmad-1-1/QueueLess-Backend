using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        /// Get list of businesses with optional filtering by category, search term, or location.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBusinesses(
            [FromQuery] string? category,
            [FromQuery] string? search,
            [FromQuery] string? location)
        {
            var businesses = await _businessRepository.GetRecommendedBusinessesAsync(category, search, location);
            return Ok(businesses);
        }

        /// <summary>
        /// Get business details by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBusinessById(Guid id)
        {
            var business = await _businessRepository.GetByIdAsync(id);
            if (business == null)
                return NotFound(new { message = "Business not found." });

            return Ok(business);
        }

        /// <summary>
        /// Get all business categories.
        /// </summary>
        [HttpGet("/api/v1/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _businessRepository.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
