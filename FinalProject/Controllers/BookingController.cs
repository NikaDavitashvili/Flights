using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;
    private readonly IBookingService _bookingService;

    public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [HttpGet("{email}")]
    [ProducesResponseType(500)]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(IEnumerable<BookingRm>), 200)]
    public async Task<ActionResult<IEnumerable<BookingRm>>> List(string email)
    {
        try
        {
            var bookings = await _bookingService.List(email);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while listing bookings");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(500)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(BookDTO dto)
    {
        try
        {
            await _bookingService.Cancel(dto);
            return CreatedAtAction(nameof(List), new { email = dto.PassengerEmail }, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while canceling booking");
            if (ex.Message.Contains("ERROR"))
                return NotFound(new { message = "Booking not found" });
            return StatusCode(500, "Internal server error");
        }
    }
}
