using System.ComponentModel.DataAnnotations;

namespace FinalProject.Domain.Models.DTOs;

public record EmailDTO(
    [Required][EmailAddress][StringLength(100, MinimumLength = 3)] string PassengerEmail);