using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using Microsoft.Extensions.DependencyInjection;

namespace FinalProject.Core.Services;
public class FlightService : IFlightService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFlightRepository _flightRepository;

    public FlightService(IServiceScopeFactory scopeFactory, IFlightRepository flightRepository)
    {
        _scopeFactory = scopeFactory;
        _flightRepository = flightRepository;
    }

    public async Task<IEnumerable<FlightRm>> Search(FlightSearchParametersDTO @params)
    {
        try
        {
            //await new FlightsScheduler(_scopeFactory).RunJobItem();

            return await _flightRepository.Search(@params) ?? new List<FlightRm>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }
    }

    /* public async Task<IEnumerable<FlightRm>> SearchTEST(FlightSearchParametersDTO @params)
    {
        var flightResults = new List<FlightRm>();
        try
        {
            string accessKey = _aviationStackSettings.AccessKey;
            string[] airlines = { "Wizz Air", "Pegasus", "LOT", "Qatar Airways", "Turkish Airlines", "American Airlines", "Lufthansa" };

            foreach (var airline in airlines)
            {
                int page = 0;
                bool hasMoreData = true;

                //while (hasMoreData)
                {
                    string url = $"https://api.aviationstack.com/v1/flights?access_key={accessKey}&airline={airline}&flight_status=active&page={page}&limit=1";

                    using (var client = new HttpClient())
                    {
                        var response = await client.GetStringAsync(url);
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response);

                        if (apiResponse != null && apiResponse.Data != null)
                        {
                            foreach (var flight in apiResponse.Data)
                            {
                                var flightRm = new FlightRm(
                                    Guid.NewGuid(),
                                    flight.Airline.Name,
                                    "100",
                                    new TimePlaceRm(flight.Departure.Airport, flight.Departure.Estimated),
                                    new TimePlaceRm(flight.Arrival.Airport, flight.Arrival.Estimated),
                                    flight.RemainingNumberOfSeats
                                );
                                flightResults.Add(flightRm);
                            }

                            hasMoreData = apiResponse.Pagination.Count > 0;
                            page++;
                        }
                        else
                        {
                            hasMoreData = false;
                        }
                    }
                }
            }

            // Save the results to a JSON file
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "flightResults.json");
            var cacheData = new FlightCache
            {
                CachedTime = DateTime.Now,
                FlightResults = flightResults
            };
            var jsonData = JsonConvert.SerializeObject(cacheData);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }

        return flightResults;
    } */

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

            if (currentMonth == m1 || currentMonth == m2 || currentMonth == m3)
                return await _flightRepository.SearchByCurrentSeason(m1, m2, m3, currentYear);

            return await _flightRepository.SearchBySeason(m1, m2, m3);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }
    }

    public async Task<List<FlightRm>> Find(string email)
    {
        try
        {
            return await _flightRepository.Find(email);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while finding the flight: {ex.Message}");
        }
    }

    public async Task<string> Book(BookDTO dto, FlightRm flight)
    {
        try
        {
            string result = await _flightRepository.Book(dto, flight);

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
public class ApiResponse
{
    public Pagination Pagination { get; set; }
    public List<FlightData> Data { get; set; }
}

public class Pagination
{
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
    public int Total { get; set; }
}

public class FlightData
{
    public Airline Airline { get; set; }
    public Departure Departure { get; set; }
    public Arrival Arrival { get; set; }
    public int RemainingNumberOfSeats { get; set; } // Assuming this field exists, adapt as needed
}

public class Airline
{
    public string Name { get; set; }
}

public class Departure
{
    public string Airport { get; set; }
    public DateTime Estimated { get; set; }
}

public class Arrival
{
    public string Airport { get; set; }
    public DateTime Estimated { get; set; }
}