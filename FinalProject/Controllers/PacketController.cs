using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;

namespace FinalProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacketController : ControllerBase
{
    private readonly IPacketService _packetService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;

    public PacketController(IPacketService packetService, IHttpContextAccessor httpContextAccessor, IUserContext userContext)
    {
        _packetService = packetService;
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
    }

    [HttpGet("{email}")]
    public async Task<ActionResult<PacketResponseDto>> GetPackets(string email)
    {
        string userId = string.Empty;
        string userEmail = string.Empty;
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

        var responseDto = new PacketResponseDto
        {
            Packets = await _packetService.GetPackets(),
            PacketId = await _packetService.GetCurrentPacketId(email)
        };

        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Get All Packets");
        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<ActionResult<PacketRm>> BuyPacket(PacketDTO Packet)
    {
        string userId = string.Empty;
        string userEmail = string.Empty;
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            userId = Guid.NewGuid().ToString();
            _userContext.UserId = userId;
            _userContext.Email = Packet.PassengerEmail;
        }
        else
        {
            userId = _userContext.UserId;
            userEmail = _userContext.Email;
        }

        var packet = await _packetService.BuyPacket(Packet);
        _httpContextAccessor.HttpContext?.Items.Add("Email", userEmail);
        _httpContextAccessor.HttpContext?.Items.Add("UserId", userId);
        _httpContextAccessor.HttpContext?.Items.Add("Action", $"Buy Packet '{packet.Name}'");
        return packet;
    }
}
