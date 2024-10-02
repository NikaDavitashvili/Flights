using FinalProject.Domain.Models.DTOs;

public interface IPassengerRepository
{
    Task<Dictionary<int, string>> AddPassenger(PassengerDTO passenger);
    Task<EmailDTO> FindPassengerByEmail(string passengerEmail);
    Task<UsernameDTO> FindPassengerByUsername(string passengerUsername);
    Task<PassengerDTO> GetPassengerByEmailAndPassword(string email, string passwordHash);

    // Add method to update the passenger details (specifically for IsVerified)
    Task VerifyPassenger(string passengerEmail);
    Task StoreEmailVerificationToken(string token, string email, DateTime expiryDate);
    Task<string> GetEmailByVerificationToken(string token);
    Task RemoveEmailVerificationToken(string token);
}