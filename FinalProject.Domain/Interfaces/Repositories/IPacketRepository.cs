using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IPacketRepository
{
    Task<IEnumerable<PacketRm>> GetPackets();
    Task<int> GetCurrentPacketId(string userEmail);
    Task<Dictionary<int, string>> UpdatePassengerPacket(PacketDTO Packet);
}
