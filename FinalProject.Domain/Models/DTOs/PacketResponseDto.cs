using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Models.DTOs;
public class PacketResponseDto
{
    public IEnumerable<PacketRm> Packets { get; set; }
    public int PacketId { get; set; }
}
