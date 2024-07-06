using FinalProject.Domain.Models.DTOs;

namespace FinalProject.Domain.Interfaces.Services;
public interface IPassengerService
{
    Task Register(NewPassengerDTO dto);
    Task<UserDTO?> Login(string email, string password);
}
