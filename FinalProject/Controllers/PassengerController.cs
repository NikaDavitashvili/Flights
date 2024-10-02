using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class PassengerController : ControllerBase
{
    private readonly IPassengerService _passengerService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;

    public PassengerController(IPassengerService passengerService, IHttpContextAccessor httpContextAccessor, IUserContext userContext)
    {
        _passengerService = passengerService;
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Register(NewPassengerDTO dto)
    {
        string userId = string.Empty;
        string email = string.Empty;
        _userContext.Email = dto.Email;
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            userId = Guid.NewGuid().ToString();
            _userContext.UserId = userId;
            _userContext.Email = dto.Email;
        }
        else
        {
            userId = _userContext.UserId;
            email = _userContext.Email;
        }

        try
        {
            await _passengerService.Register(dto);
        }
        catch (Exception ex)
        {
            if (ex.Message == "This email and username is already registered!" ||
                ex.Message == "This email is already registered!" ||
                ex.Message == "This username is already registered!")
            {
                return Conflict(new { message = ex.Message });
            }
            return StatusCode(500, new { message = "Registration failed!" });
        }

        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", email);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Register User - {dto.Email}");
        return Ok();
    }


    [HttpPost("{email}&{password}")]
    public async Task<ActionResult<UserDTO>> Login(string email, string password)
    {
        string userId = string.Empty;
        string userEmail = string.Empty;
        _userContext.Email = email;
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            userId = Guid.NewGuid().ToString();
            _userContext.UserId = userId;
            _userContext.Email = email;
        }
        else
        {
            userId = _userContext.UserId;
            userEmail = _userContext.Email;
        }

        var userDto = await _passengerService.Login(email, password);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Login User - {email}");
        return Ok(userDto);
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = Guid.NewGuid().ToString();
        _httpContextAccessor.HttpContext?.Items.Add("UserId", _userContext.UserId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", _userContext.Email);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Logout User");
        _userContext.Email = string.Empty;
        _userContext.UserId = userId;

        return Ok();
    }
}



/*using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class PassengerController : ControllerBase
{
    private readonly IPassengerService _passengerService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PassengerController(IPassengerService passengerService, IHttpContextAccessor httpContextAccessor)
    {
        _passengerService = passengerService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Register(NewPassengerDTO dto)
    {
        await _passengerService.Register(dto);
        return Ok();
    }

    [HttpPost("login/{email}&{password}")]
    public async Task<ActionResult<UserDTO>> Login(string email, string password)
    {
        var userDto = await _passengerService.Login(email, password);
        _httpContextAccessor.HttpContext?.Items.Add("Email", email);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Login User - {email}");
        return Ok(userDto);
    }

    [HttpPost("logout/{email}&{password}")]
    public async Task LogOut(string email, string password)
    {
        _httpContextAccessor.HttpContext?.Items.Remove("UserId");
        _httpContextAccessor.HttpContext?.Items.Remove("Email");
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Logout User - {email}");
    }
}
*/