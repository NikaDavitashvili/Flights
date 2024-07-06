using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.Data;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly Entities _entities;
    private readonly IBookingService _bookingService;

    public BookingController(Entities entities, IBookingService bookingService)
    {
        _entities = entities;
        _bookingService = bookingService;
    }

    [HttpGet("{email}")]
    [ProducesResponseType(500)]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(IEnumerable<BookingRm>), 200)]
    public async Task<ActionResult<IEnumerable<BookingRm>>> List(string email)
    {
        var bookings = await _bookingService.List(email);

        return Ok(bookings);
    }


    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(500)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(BookDTO dto)
    {
        var flight = _entities.Flights.Find(dto.FlightId);

        var error = flight?.CancelBooking(dto.PassengerEmail, dto.NumberOfSeats);

        if (error == null)
        {
            _entities.SaveChanges();
            return NoContent();
        }

        if (error is NotFoundErrorDTO)
            return NotFound();

        throw new Exception($"The error of type: {error.GetType().Name} occurred while canceling the booking made by {dto.PassengerEmail}");
    }

}
