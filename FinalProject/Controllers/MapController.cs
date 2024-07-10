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

    [HttpGet]
    public async Task<ActionResult<CitiesRm>> GetCities()
    {
        try
        {
            var cities = await _mapService.GetCities();

            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while listing cities");
            return StatusCode(500, "Internal server error");
        }
    }
}
