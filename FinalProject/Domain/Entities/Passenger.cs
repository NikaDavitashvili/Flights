namespace Flights.Domain.Entities
{
    public record Passenger(
        string Email,
        string PasswordHash,
        string UserName,
        string FirstName,
        string LastName,
        string Gender);
}
