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
        /// Gets all data required for the Home Screen.
        /// </summary>
        /// <param name="latitude">
        /// User's current latitude obtained from the mobile device.
        /// </param>
        /// <param name="longitude">
        /// User's current longitude obtained from the mobile device.
        /// </param>
        [HttpGet]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(HomePageDataResponse))]
        public async Task<IActionResult> GetHomePageData(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude)
        {
            var data = await _homeService.GetHomePageDataAsync(
                latitude,
                longitude);

            return Ok(data);
        }
    }
}