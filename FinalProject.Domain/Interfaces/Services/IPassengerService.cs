using FinalProject.Domain.Models.DTOs;

public interface IPassengerService
{
    Task Register(NewPassengerDTO dto);
    Task<UserDTO> Login(string email, string password);
    Task<string> GenerateEmailVerificationToken(string email);  // Generate verification token
    Task SendVerificationEmail(string email, string userName, string verificationUrl);  // Send verification email
    Task<bool> VerifyEmailToken(string token);  // Verify the token and set IsVerified to true
}