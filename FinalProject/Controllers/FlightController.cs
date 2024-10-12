using FinalProject.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using FinalProject.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FlightController(IFlightService flightService, IHttpContextAccessor httpContextAccessor, IUserContext userContext)
    {
        _flightService = flightService;
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
    }

    [HttpGet]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(IEnumerable<FlightRm>), 200)]
    public async Task<IEnumerable<FlightRm>> Search([FromQuery] FlightSearchParametersDTO @params)
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

        if (!string.IsNullOrEmpty(@params.SeasonName))
        {
            var discountedFlightsBySeasons = await _flightService.SearchBySeason(@params.SeasonName);
            _httpContextAccessor.HttpContext?.Items.Add("Action", $"Searching Flights By Season '{@params.SeasonName}'");
            _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
            _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
            return discountedFlightsBySeasons;
        }

        var flights = await _flightService.Search(@params);
        var flights = await _flightService.SearchAviaSales(@params);
        _httpContextAccessor.HttpContext?.Items.Add("Action", "Searching Flights");
        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);

        return flights;
    }

    /* [HttpGet]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(IEnumerable<FlightRm>), 200)]
    public async Task<IEnumerable<FlightRm>> Search([FromQuery] FlightSearchParametersDTO @params)
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

        // Check if data exists in the JSON file
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "flightResults.json");
        if (System.IO.File.Exists(jsonFilePath))
        {
            var jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            var cachedData = JsonConvert.DeserializeObject<FlightCache>(jsonData);

            if (cachedData != null && cachedData.FlightResults.Any())
            {
                if ((DateTime.Now - cachedData.CachedTime).TotalHours < 24 )
                {
                    _httpContextAccessor.HttpContext?.Items.Add("Action", "Retrieving Cached Flights");
                    _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
                    _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
                    return cachedData.FlightResults;
                }
            }
        }
        
        if (!string.IsNullOrEmpty(@params.SeasonName))
        {
            var discountedFlightsBySeasons = await _flightService.SearchBySeason(@params.SeasonName);
            _httpContextAccessor.HttpContext?.Items.Add("Action", $"Searching Flights By Season '{@params.SeasonName}'");
            _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
            _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
            return discountedFlightsBySeasons;
        }

        // If no valid cache, call SearchTEST and fetch new data
        var flights = await _flightService.SearchTEST(@params);
        _httpContextAccessor.HttpContext?.Items.Add("Action", "Searching Flights");
        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);

        return flights;
    } */

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(FlightRm), 200)]
    public async Task<ActionResult<FlightRm>> Find(string email)
    public async Task<ActionResult<FlightRm>> Find(Guid id)
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

        var flight = await _flightService.Find(email);
        var flight = await _flightService.Find(id);
        if (flight == null)
            return NotFound();

        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Find Flight With Email - {email}");
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Find Flight Id {id}");
        return Ok(flight);
    }

    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Book(FlightRm dto)
    public async Task<IActionResult> Book(BookDTO dto)
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

        var bookDto = new BookDTO(Guid.NewGuid(), userEmail, (byte)dto.SeatsToBuy);
        string result = await _flightService.Book(bookDto, dto);
        string result = await _flightService.Book(dto);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Booking a New Flight With From '{dto.Departure.Place}' To '{dto.Arrival.Place}'");
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Booking a New Flight With Id - {dto.FlightId}");
        HttpContext.Session.SetString("TicketAmountErrorMessage", result);
        return Ok(dto);
        //return CreatedAtAction(nameof(Find), new { email = userEmail }, null);
        return CreatedAtAction(nameof(Find), new { id = dto.FlightId }, null);
    }
}
