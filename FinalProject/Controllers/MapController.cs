using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.ReadModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MapController : ControllerBase
{
    private readonly IMapService _mapService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;
//sdsd
    public MapController(IMapService mapService, IHttpContextAccessor httpContextAccessor, IUserContext userContext)
    {
        _mapService = mapService;
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
    }

    [HttpGet("{departureCity}&{arrivalCity}")]
    public async Task<ActionResult<CitiesRm>> GetCities(string departureCity, string arrivalCity)
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

        var cities = new List<CitiesRm>();
        if (string.IsNullOrEmpty(departureCity) || departureCity == "undefined" ||
            string.IsNullOrEmpty(arrivalCity) || arrivalCity == "undefined")
        {
            cities = (await _mapService.GetCities()).ToList();

            _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
            _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
            _httpContextAccessor.HttpContext?.Items.Add("Action", $"Map Searching");
            return Ok(cities);
        }

        var optimalTripRoute = (await _mapService.GetOptimalTripRoute(departureCity, arrivalCity)).ToList();

        if (optimalTripRoute == null || optimalTripRoute.Count == 0)
        {
            _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
            _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
            _httpContextAccessor.HttpContext?.Items.Add("Action", $"Map Searching");
            return Ok(cities);
        }

        foreach (var trip in optimalTripRoute)
        {
            cities.Add(new CitiesRm(trip!.DepartureCity, trip!.ArrivalCity, trip!.Price));
        }

        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Map Searching");
        return Ok(cities);
    }
}
