using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Services;
public interface IMapService
{
    Task<IEnumerable<CitiesRm>> GetCities();
}
