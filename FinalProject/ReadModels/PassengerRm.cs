namespace Flights.ReadModels
{
    public record PassengerRm(
        string Email,
        string PasswordHash,
        string UserName,
        string FirstName,
        string LastName,
        string Gender);

}
