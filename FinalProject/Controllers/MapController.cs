using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.ReadModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MapController : ControllerBase
{
    private readonly ILogger<MapController> _logger;
    private readonly IMapService _mapService;

    public MapController(ILogger<MapController> logger, IMapService mapService)
    {
        _logger = logger;
        _mapService = mapService;
    }

    [HttpGet("{departureCity}&{arrivalCity}")]
    public async Task<ActionResult<CitiesRm>> GetCities(string departureCity, string arrivalCity)
    {
        try
        {
            var cities = new List<CitiesRm>();
            if (string.IsNullOrEmpty(departureCity) || departureCity == "undefined" ||
                string.IsNullOrEmpty(arrivalCity)   || arrivalCity == "undefined")
            {
                cities = (await _mapService.GetCities()).ToList();

                return Ok(cities);
            }

            var optimalTripRoute = (await _mapService.GetOptimalTripRoute(departureCity, arrivalCity)).ToList();

            if (optimalTripRoute == null || optimalTripRoute.Count == 0)
                return Ok(cities);

            foreach (var trip in optimalTripRoute)
            {
                cities.Add(new CitiesRm(trip!.DepartureCity, trip!.ArrivalCity, trip!.Price)); 
            }

            return Ok(cities);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while listing cities");
            return StatusCode(500, "Internal server error");
        }
    }
}
