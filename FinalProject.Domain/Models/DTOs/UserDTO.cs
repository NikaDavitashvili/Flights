namespace FinalProject.Domain.Models.DTOs;
public record UserDTO(
    string Email,
    string PasswordHash,
    string UserName,
    int PacketId,
    int PurchasePercent,
    int CancelPercent
    );
