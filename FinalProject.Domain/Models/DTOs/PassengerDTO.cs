namespace FinalProject.Domain.Models.DTOs;
public record PassengerDTO(
    string Email,
    string PasswordHash,
    string UserName,
    string FirstName,
    string LastName,
    string Gender,
    int PacketID,
    int PurchasePercent,
    int CancelPercent);
