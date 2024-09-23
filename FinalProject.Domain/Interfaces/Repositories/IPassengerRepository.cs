using FinalProject.Domain.Models.DTOs;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IPassengerRepository
{
    Task<Dictionary<int, string>> AddPassenger(PassengerDTO passenger);
    Task<PassengerDTO> GetPassengerByEmailAndPassword(string email, string passwordHash);
}
