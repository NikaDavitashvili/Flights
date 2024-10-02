using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Interfaces.Services;

namespace FinalProject.Controllers;
[ApiController]
[Route("[controller]")]
public class PassengerController : Controller
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
        _userContext.Email = dto.Email.Trim();

        // Generate user ID and set email in the user context if necessary
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            userId = Guid.NewGuid().ToString();
            _userContext.UserId = userId;
            _userContext.Email = dto.Email.Trim();
        }
        else
        {
            userId = _userContext.UserId;
            email = _userContext.Email;
        }

        try
        {
            // Call the service to register the user with IsVerified = false
            await _passengerService.Register(dto);
            
            string token = await _passengerService.GenerateEmailVerificationToken(dto.Email.Trim());
            string verificationUrl = Url.Action("VerifyEmail", "Passenger", new { token = token }, Request.Scheme);
            await _passengerService.SendVerificationEmail(dto.Email, dto.UserName, verificationUrl);
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
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Register User - {dto.Email.Trim()}");
        return Ok(new { message = "Registration successful. Please verify your email." });
    }

    [HttpGet("VerifyEmail")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        try
        {
            var isTokenValid = await _passengerService.VerifyEmailToken(token);

            if (!isTokenValid)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }

            //return View("VerifyEmail");
            return Ok(new { message = "Email verification successful. You can now log in." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Verification failed!" });
        }
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

        if (!userDto.IsVerified) 
            return Unauthorized(new { message = "Please verify your email before logging in." });

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