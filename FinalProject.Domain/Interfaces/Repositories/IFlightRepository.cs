using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IFlightRepository
{
    Task<IEnumerable<FlightRm>?> Search(FlightSearchParametersDTO @params);
    Task<IEnumerable<FlightRm>> SearchByCurrentSeason(int m1, int m2, int m3, int currentYear);
    Task<IEnumerable<FlightRm>> SearchBySeason(int m1, int m2, int m3);
    Task<FlightRm> Find(Guid id);
    Task<string> Book(BookDTO dto);
}

