using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class PassengerController : ControllerBase
{
    private readonly ILogger<PassengerController> _logger;
    private readonly IPassengerService _passengerService;

    public PassengerController(ILogger<PassengerController> logger, IPassengerService passengerService)
    {
        _logger = logger;
        _passengerService = passengerService;
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Register(NewPassengerDTO dto)
    {
        try
        {
            await _passengerService.Register(dto);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering passenger");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{email}&{password}")]
    public async Task<ActionResult<UserDTO>> Login(string email, string password)
    {
        try
        {
            var userDto = await _passengerService.Login(email, password);
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging in");
            return NotFound();
        }
    }
}