using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IFlightRepository
{
    Task<IEnumerable<FlightRm>?> Search(FlightSearchParametersDTO @params);
    //Task<IEnumerable<FlightRm>> Search(FlightSearchParametersDTO @params);
    Task<IEnumerable<FlightRm>> SearchByCurrentSeason(int m1, int m2, int m3, int currentYear);
    Task<IEnumerable<FlightRm>> SearchBySeason(int m1, int m2, int m3);
    Task<List<FlightRm>> Find(string email);
    Task<string> Book(BookDTO dto, FlightRm flight);
    Task<List<string>> GetAirportIataCodes(string searchWord);
    Task<string> GetAirportName(string airportCode);
    Task<string> GetCityName(string cityCode);
    Task<string> GetCountryName(string countryCode);
    //Task<FlightRm> Find(Guid id);
    //Task<string> Book(BookDTO dto);
}

