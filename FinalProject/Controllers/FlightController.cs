using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProject.Domain.Models.Data;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private readonly ILogger<FlightController> _logger;

    private readonly Entities _entities;
    private readonly IFlightService _flightService;


    public FlightController(ILogger<FlightController> logger, Entities entities, IFlightService flightService)
    {
        _logger = logger;
        _entities = entities;
        _flightService = flightService;
    }

    [HttpGet]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(IEnumerable<FlightRm>), 200)]
    public async Task<IEnumerable<FlightRm>> Search([FromQuery] FlightSearchParametersDTO @params)
    {

        _logger.LogInformation("Searching for a flight for: {Destination}", @params.Destination);

        IQueryable<Flight> flights = _entities.Flights;

        if (!string.IsNullOrWhiteSpace(@params.Destination))
            flights = flights.Where(f => f.Arrival.Place.Contains(@params.Destination));

        if (!string.IsNullOrWhiteSpace(@params.From))
            flights = flights.Where(f => f.Departure.Place.Contains(@params.From));

        if (@params.FromDate != null)
            flights = flights.Where(f => f.Departure.Time >= @params.FromDate.Value.Date);

        if (@params.ToDate != null)
            flights = flights.Where(f => f.Departure.Time >= @params.ToDate.Value.Date.AddDays(1).AddTicks(-1));

        if (@params.NumberOfPassengers != 0 && @params.NumberOfPassengers != null)
            flights = flights.Where(f => f.RemainingNumberOfSeats >= @params.NumberOfPassengers);
        else
            flights = flights.Where(f => f.RemainingNumberOfSeats >= 1);


        var flightRmList = flights
            .Select(flight => new FlightRm(
            flight.Id,
            flight.Airline,
            flight.Price,
            new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time),
            new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
            flight.RemainingNumberOfSeats
            ));

        return flightRmList;
    }


    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(FlightRm), 200)]
    public async Task<ActionResult<FlightRm>> Find(Guid id)
    {
        var flight = _entities.Flights.SingleOrDefault(f => f.Id == id);

        if (flight == null)
            return NotFound();

        var readModel = new FlightRm(
            flight.Id,
            flight.Airline,
            flight.Price,
            new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time),
            new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
            flight.RemainingNumberOfSeats
            );

        return Ok(readModel);
    }

    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Book(BookDTO dto)
    {
        System.Diagnostics.Debug.WriteLine($"Booking a new flight {dto.FlightId}");

        var flight = _entities.Flights.SingleOrDefault(f => f.Id == dto.FlightId);

        if (flight == null)
            return NotFound();

        var error = flight.MakeBooking(dto.PassengerEmail, dto.NumberOfSeats);

        if (error is OverbookErrorDTO)
            return Conflict(new { message = "Not enough seats." });


        try
        {
            _entities.SaveChanges();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return Conflict(new { message = "An error occurred while booking. Please try again." });
        }

        return CreatedAtAction(nameof(Find), new { id = dto.FlightId });
    }

}