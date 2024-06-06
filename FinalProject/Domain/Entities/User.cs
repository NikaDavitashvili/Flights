namespace FinalProject.Domain.Entities
{
    public record User(
        string Email,
        string PasswordHash,
        string UserName);
}
