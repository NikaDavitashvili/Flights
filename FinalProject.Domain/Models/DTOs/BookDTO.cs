using System.ComponentModel.DataAnnotations;

namespace FinalProject.Domain.Models.DTOs;
public record BookDTO(
    [Required] Guid FlightId,
    [Required][EmailAddress][StringLength(100, MinimumLength = 3)] string PassengerEmail,
    [Required][Range(1,10)] byte NumberOfSeats);

