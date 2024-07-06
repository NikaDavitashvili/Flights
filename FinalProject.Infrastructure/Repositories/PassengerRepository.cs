using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;

namespace FinalProject.Infrastructure.Repositories;
public class PassengerRepository : IPassengerRepository
{
    public Task<UserDTO?> Login(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task Register(NewPassengerDTO dto)
    {
        throw new NotImplementedException();
    }
}
