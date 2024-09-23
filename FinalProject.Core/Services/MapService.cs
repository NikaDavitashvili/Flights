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

    public async Task<IEnumerable<OptimalFlightNodeRm?>> GetOptimalTripRoute(string departureCity, string arrivalCity)
    {
        var flights = await _mapRepository.GetOptimalTripRoute(departureCity, arrivalCity);

        if (flights == null || flights.Count() == 0)
            return new List<OptimalFlightNodeRm>();

        var priorityQueue = new PriorityQueue<Node, decimal>();
        var visited = new HashSet<string>();

        priorityQueue.Enqueue(new Node(departureCity, 0, new List<OptimalFlightNodeRm>()), 0);

        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue();

            if (visited.Contains(currentNode.City))
                continue;

            visited.Add(currentNode.City);

            if (currentNode.City == arrivalCity)
                return currentNode.Path;

            var nextFlights = flights.Where(f => f.DepartureCity == currentNode.City).OrderBy(f => f.Price);

            foreach (var flight in nextFlights)
            {
                if (!visited.Contains(flight.ArrivalCity))
                {
                    var newPath = new List<OptimalFlightNodeRm>(currentNode.Path) { flight };
                    var newNode = new Node(flight.ArrivalCity, currentNode.Price + flight.Price, newPath);
                    priorityQueue.Enqueue(newNode, newNode.Price);
                }
            }
        }

        return new List<OptimalFlightNodeRm>();
    }
}
