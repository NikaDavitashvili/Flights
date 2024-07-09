using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Services;
public interface IPacketService
{
    Task<IEnumerable<PacketRm>> GetPackets();
    Task<int> GetCurrentPacketId(string userEmail);
    Task<Dictionary<int, string>> BuyPacket(PacketDTO Packet);
}
