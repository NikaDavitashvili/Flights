using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IMapRepository
{
    Task<IEnumerable<CitiesRm>> GetCities();
    Task<IEnumerable<OptimalFlightNodeRm>> GetOptimalTripRoute(string departureCity, string arrivalCity);
}
