using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;

    public BookingController(IBookingService bookingService, IHttpContextAccessor httpContextAccessor, IUserContext userContext)
    {
        _bookingService = bookingService;
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
    }

    [HttpGet("{email}")]
    [ProducesResponseType(500)]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(IEnumerable<BookingRm>), 200)]
    public async Task<ActionResult<IEnumerable<BookingRm>>> List(string email)
    {
        string userId = string.Empty;
        string userEmail = string.Empty;
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            userId = Guid.NewGuid().ToString();
            _userContext.UserId = userId;
        }
        else
        {
            userId = _userContext.UserId;
            userEmail = _userContext.Email;
        }

        var bookings = await _bookingService.List(email);

        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Booked Flights On Email '{email}'");
        return Ok(bookings);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(500)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(BookDTO dto)
    {
        string userId = string.Empty;
        string userEmail = string.Empty;
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            userId = Guid.NewGuid().ToString();
            _userContext.UserId = userId;
        }
        else
        {
            userId = _userContext.UserId;
            userEmail = _userContext.Email;
        }

        await _bookingService.Cancel(dto);
        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Cancelling Flight On Email '{dto.PassengerEmail}' With FlightId - {dto.FlightId}");
        return CreatedAtAction(nameof(List), new { email = dto.PassengerEmail }, null);
    }
}
