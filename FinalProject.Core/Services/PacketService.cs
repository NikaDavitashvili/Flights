using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Core.Services;
public class PacketService : IPacketService
{
    private readonly IPacketRepository _packetRepository;

    public PacketService(IPacketRepository packetRepository)
    {
        _packetRepository = packetRepository;
    }

    public async Task<IEnumerable<PacketRm>> GetPackets()
    {
        var packets = await _packetRepository.GetPackets();

        return packets;
    }
    public async Task<int> GetCurrentPacketId(string userEmail)
    {
        var packetId = await _packetRepository.GetCurrentPacketId(userEmail);

        return packetId;
    }

    public async Task<PacketRm> BuyPacket(PacketDTO Packet)
    {
       var result = await _packetRepository.UpdatePassengerPacket(Packet);

        return result;
    }
}
