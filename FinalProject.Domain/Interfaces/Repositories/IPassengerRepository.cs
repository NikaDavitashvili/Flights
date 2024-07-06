using FinalProject.Domain.Models.DTOs;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IPassengerRepository
{
    Task Register(NewPassengerDTO dto);
    Task<UserDTO?> Login(string email, string password);
}
