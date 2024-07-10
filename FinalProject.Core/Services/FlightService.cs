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
            return await _flightRepository.Search(@params);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }
    }
    public async Task<IEnumerable<FlightRm>> SearchBySeason(string seasonName)
    {
        try
        {
            int m1, m2, m3;
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            if (seasonName == "Spring")
            {
                m1 = 3;
                m2 = 4;
                m3 = 5;
            }
            else if (seasonName == "Summer")
            {
                m1 = 6;
                m2 = 7;
                m3 = 8;
            }
            else if (seasonName == "Autumn")
            {
                m1 = 9;
                m2 = 10;
                m3 = 11;
            }
            else if (seasonName == "Winter")
            {
                m1 = 12;
                m2 = 1;
                m3 = 2;
            }
            else
                throw new Exception();

            if(currentMonth == m1 || currentMonth == m2 || currentMonth == m3)
                return await _flightRepository.SearchByCurrentSeason(m1, m2, m3, currentYear);

            return await _flightRepository.SearchBySeason(m1, m2, m3);
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
            return await _flightRepository.Find(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while finding the flight: {ex.Message}");
        }
    }

    public async Task<string> Book(BookDTO dto)
    {
        try
        {
            string result = await _flightRepository.Book(dto);

            return result;
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
