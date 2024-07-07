using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Core.Services;
public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;

    public FlightService(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<IEnumerable<FlightRm>> Search(FlightSearchParametersDTO @params)
    {
        try
        {
            return _flightRepository.Search(@params);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }
    }

    public async Task<FlightRm> Find(Guid id)
    {
        try
        {
            return _flightRepository.Find(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while finding the flight: {ex.Message}");
        }
    }

    public async Task Book(BookDTO dto)
    {
        try
        {
            _flightRepository.Book(dto);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Flight not found"))
                throw new Exception("ERROR: Flight not found");

            if (ex.Message.Contains("Not enough seats"))
                throw new Exception("ERROR: Not enough seats");

            throw new Exception($"Error occurred while booking the flight: {ex.Message}");
        }
    }
}
