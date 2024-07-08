using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private readonly ILogger<FlightController> _logger;
    private readonly IFlightService _flightService;

    public FlightController(ILogger<FlightController> logger, IFlightService flightService)
    {
        _logger = logger;
        _flightService = flightService;
    }

    [HttpGet]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(IEnumerable<FlightRm>), 200)]
    public async Task<IEnumerable<FlightRm>> Search([FromQuery] FlightSearchParametersDTO @params)
    {

        _logger.LogInformation("Searching for a flight for: {Destination}", @params.Destination);

        try
        {
            var flights = await _flightService.Search(@params);
            return flights;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for flights");
            return new List<FlightRm>();
            //return StatusCode(500, "Internal server error");
        }
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(FlightRm), 200)]
    public async Task<ActionResult<FlightRm>> Find(Guid id)
    {
        try
        {
            var flight = await _flightService.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while finding the flight");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Book(BookDTO dto)
    {
        _logger.LogInformation($"Booking a new flight {dto.FlightId}");

        try
        {
            string result = await _flightService.Book(dto);
            HttpContext.Session.SetString("TicketAmountErrorMessage", result);
            return CreatedAtAction(nameof(Find), new { id = dto.FlightId }, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while booking the flight");
            if (ex.Message.Contains("ERROR: Flight not found"))
                return NotFound();
            if (ex.Message.Contains("ERROR: Not enough seats"))
                return Conflict(new { message = "Not enough seats." });
            return StatusCode(500, "Internal server error");
        }
    }
}
