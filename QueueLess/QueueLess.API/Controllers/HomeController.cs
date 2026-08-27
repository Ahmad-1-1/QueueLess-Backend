using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;

namespace QueueLess.API.Controllers
{
    [ApiController]
    [Route("api/v1/home")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        /// <summary>
        /// Get all data required for the Home Screen UI (Categories, Popular Services, Recommended Businesses).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HomePageDataResponse))]
        public async Task<IActionResult> GetHomePageData(
            [FromQuery] string? category,
            [FromQuery] string? search,
            [FromQuery] string? location)
        {
            var data = await _homeService.GetHomePageDataAsync(category, search, location);
            return Ok(data);
        }
    }
}
