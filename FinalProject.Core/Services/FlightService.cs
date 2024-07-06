using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.Data;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using Microsoft.Extensions.Logging;

namespace FinalProject.Core.Services;
public class FlightService : IFlightService
{
    private readonly ILogger<FlightService> _logger;

    private readonly Entities _entities;
    private readonly IFlightRepository _flightRepository;

    public FlightService(ILogger<FlightService> logger, Entities entities, IFlightRepository flightRepository)
    {
        _logger = logger;
        _entities = entities;
        _flightRepository = flightRepository;
    }

    public async Task<IQueryable<FlightRm>> Search(FlightSearchParametersDTO @params)
    {
        _logger.LogInformation("Searching for a flight for: {Destination}", @params.Destination);

        IQueryable<Flight> flights = _entities.Flights;

        if (!string.IsNullOrWhiteSpace(@params.Destination))
            flights = flights.Where(f => f.Arrival.Place.Contains(@params.Destination));

        if (!string.IsNullOrWhiteSpace(@params.From))
            flights = flights.Where(f => f.Departure.Place.Contains(@params.From));

        if (@params.FromDate != null)
            flights = flights.Where(f => f.Departure.Time >= @params.FromDate.Value.Date);

        if (@params.ToDate != null)
            flights = flights.Where(f => f.Departure.Time >= @params.ToDate.Value.Date.AddDays(1).AddTicks(-1));

        if (@params.NumberOfPassengers != 0 && @params.NumberOfPassengers != null)
            flights = flights.Where(f => f.RemainingNumberOfSeats >= @params.NumberOfPassengers);
        else
            flights = flights.Where(f => f.RemainingNumberOfSeats >= 1);


        var flightRmList = flights
            .Select(flight => new FlightRm(
            flight.Id,
            flight.Airline,
            flight.Price,
            new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time),
            new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
            flight.RemainingNumberOfSeats
            ));

        return flightRmList;
    }

    public async Task<FlightRm?> Find(Guid id)
    {
        var flight = _entities.Flights.SingleOrDefault(f => f.Id == id);

        if (flight == null)
            return null;

        var readModel = new FlightRm(
            flight.Id,
            flight.Airline,
            flight.Price,
            new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time),
            new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
            flight.RemainingNumberOfSeats
            );

        return readModel;
    }

    public async Task<string> Book(BookDTO dto)
    {
        var flight = _entities.Flights.SingleOrDefault(f => f.Id == dto.FlightId);

        if (flight == null)
            return "Flights Not Found!";

        var error = flight.MakeBooking(dto.PassengerEmail, dto.NumberOfSeats);

        if (error is OverbookErrorDTO)
            return "Not Enough Seats!";


        try
        {
            _entities.SaveChanges();
        }
        catch (Exception e)
        {
            return "An error occurred while booking. Please try again.";
        }

        return "Success";
    }
}
