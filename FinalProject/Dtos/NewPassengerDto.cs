using System.ComponentModel.DataAnnotations;

namespace Flights.Dtos
{
    public record NewPassengerDto(
        [Required][EmailAddress][StringLength(100, MinimumLength = 3)] string Email,
        [Required][MinLength(5)][MaxLength(25)] string Password,
        [Required][MinLength(3)][MaxLength(50)] string UserName,
        [Required][MinLength(2)][MaxLength(35)] string FirstName,
        [Required][MinLength(2)][MaxLength(35)] string LastName,
        [Required] bool Gender);
}
