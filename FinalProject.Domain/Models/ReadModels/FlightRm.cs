namespace FinalProject.Domain.Models.ReadModels;
public record FlightRm(
    Guid Id,
    string Airline,
    string Link,
    string Price,
    TimePlaceRm Departure,
    TimePlaceRm Arrival,
    int RemainingNumberOfSeats
    );
