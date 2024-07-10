using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Core.Services;
public class MapService : IMapService
{
    private readonly IMapRepository _mapRepository;

    public MapService(IMapRepository mapRepository)
    {
        _mapRepository = mapRepository;
    }
    public async Task<IEnumerable<CitiesRm>> GetCities()
    {
        var cities = await _mapRepository.GetCities();

        return cities;
    }
}
