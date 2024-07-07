using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;

namespace FinalProject.Core.Services;
public class PassengerService : IPassengerService
{
    private readonly IPassengerRepository _passengerRepository;
    private readonly IHelper _helper;

    public PassengerService(IPassengerRepository passengerRepository, IHelper helper)
    {
        _passengerRepository = passengerRepository;
        _helper = helper;
    }

    public async Task Register(NewPassengerDTO dto)
    {
        var passwordHash = _helper.PasswordHash(dto.Password);
        var gender = dto.Gender ? "Female" : "Male";
        var passenger = new PassengerDTO(
            dto.Email,
            passwordHash,
            dto.UserName,
            dto.FirstName,
            dto.LastName,
            gender
        );

        var result = await _passengerRepository.AddPassenger(passenger);

        if (result.Values.Any(message => message.StartsWith("The input string '")))
        {
            throw new Exception("Registration Failed!");
        }
    }

    public async Task<UserDTO> Login(string email, string password)
    {
        var passwordHash = _helper.PasswordHash(password);
        var passenger = await _passengerRepository.GetPassengerByEmailAndPassword(email, passwordHash);

        if (passenger == null)
        {
            throw new Exception("Passenger not found");
        }

        return new UserDTO(
            passenger.Email,
            passenger.PasswordHash,
            passenger.UserName
        );
    }
}

