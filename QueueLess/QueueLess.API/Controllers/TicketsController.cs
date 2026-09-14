using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;

namespace QueueLess.API.Controllers
{
    [ApiController]
    [Route("api/v1/tickets")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException("Invalid token."));

        /// <summary>
        /// Reserve/Book a new digital queue ticket for a specific service.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TicketResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketRequest request)
        {
            var response = await _ticketService.BookTicketAsync(CurrentUserId, request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Get the user's currently active ticket (Waiting or Serving), with live queue details.
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TicketResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveTicket()
        {
            var response = await _ticketService.GetActiveTicketAsync(CurrentUserId);
            return Ok(response);
        }

        /// <summary>
        /// Refresh the live queue status for a specific ticket (People Ahead, Wait Time, Serving).
        /// </summary>
        [HttpGet("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LiveQueueStatusDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQueueStatus(Guid id)
        {
            var response = await _ticketService.GetQueueStatusAsync(id, CurrentUserId);
            return Ok(response);
        }

        /// <summary>
        /// Cancel a waiting ticket. Allowed only if there are >= 10 people ahead.
        /// </summary>
        [HttpPut("{id:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelTicket(Guid id)
        {
            await _ticketService.CancelTicketAsync(id, CurrentUserId);
            return Ok(new { message = "Ticket successfully cancelled." });
        }
    }
}
