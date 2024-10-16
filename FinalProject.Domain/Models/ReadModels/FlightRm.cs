﻿namespace FinalProject.Domain.Models.ReadModels;
public record FlightRm(
    Guid Id,
    string Airline,
    string Price,
    TimePlaceRm Departure,
    TimePlaceRm Arrival,
    int RemainingNumberOfSeats,
    int SeatsToBuy
    );
