using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.Data;
using FinalProject.Domain.Models.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Core.Services;
public class PassengerService : IPassengerService
{
    private readonly Entities _entities;
    private readonly IPassengerRepository _passengerRepository;

    public PassengerService(Entities entities, IPassengerRepository passengerRepository)
    {
        _entities = entities;
        _passengerRepository = passengerRepository;
    }

    public async Task Register(NewPassengerDTO dto)
    {
        var password = PasswordHash(dto.Password);
        var gender = dto.Gender ? "Female" : "Male";
        _entities.Passengers.Add(new Passenger(
            dto.Email,
            password,
            dto.UserName,
            dto.FirstName,
            dto.LastName,
            gender
            ));

        _entities.SaveChanges();
    }

    public async Task<UserDTO?> Login(string email, string password)
    {
        var passwordHash = PasswordHash(password);
        var passenger = _entities.Passengers.FirstOrDefault(p => p.Email == email && p.PasswordHash == passwordHash);

            if (passenger == null)
                return null;

            var rm = new UserDTO(
                passenger.Email,
                passenger.PasswordHash,
                passenger.UserName
            );

            return rm;
    }

    private string PasswordHash(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
