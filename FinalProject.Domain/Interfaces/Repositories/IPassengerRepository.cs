using FinalProject.Domain.Models.DTOs;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IPassengerRepository
{
    Task<Dictionary<int, string>> AddPassenger(PassengerDTO passenger);
    Task<EmailDTO> FindPassengerByEmail(string passengerEmail);
    Task<UsernameDTO> FindPassengerByUsername(string passengerUsername);
    Task<PassengerDTO> GetPassengerByEmailAndPassword(string email, string passwordHash);
}
