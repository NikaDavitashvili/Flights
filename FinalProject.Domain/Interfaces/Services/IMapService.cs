using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Services;
public interface IMapService
{
    Task<IEnumerable<CitiesRm>> GetCities();
    Task<IEnumerable<OptimalFlightNodeRm?>> GetOptimalTripRoute(string departureCity, string arrivalCity);
}
