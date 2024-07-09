using Microsoft.AspNetCore.Mvc;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using System.Collections.Generic;

namespace FinalProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacketController : ControllerBase
{
    private readonly ILogger<PacketController> _logger;
    private readonly IPacketService _packetService;

    public PacketController(ILogger<PacketController> logger, IPacketService packetService)
    {
        _logger = logger;
        _packetService = packetService;
    }

    [HttpGet("{email}")]
    public async Task<ActionResult<PacketResponseDto>> GetPackets(string email)
    {
        try
        {
            var responseDto = new PacketResponseDto
            {
                Packets = await _packetService.GetPackets(),
                PacketId = await _packetService.GetCurrentPacketId(email)
            };

            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while listing packets");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<PacketRm>> BuyPacket(PacketDTO Packet)
    {
        try
        {
            var packet = await _packetService.BuyPacket(Packet);
            return packet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while buying packet");
            return StatusCode(500, "Internal server error");
        }
    }
}
