
using System.ComponentModel.DataAnnotations;

public record UsernameDTO(
    [Required][MinLength(3)][MaxLength(50)] string PassengerUsername);