using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FinalProject.Core.Services;
public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;
    private readonly AviationStackSettings _aviationStackSettings;

    public FlightService(IFlightRepository flightRepository, IOptions<AviationStackSettings> aviationStackSettings)
    {
        _flightRepository = flightRepository;
        _aviationStackSettings = aviationStackSettings.Value;
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


    public async Task<IEnumerable<FlightRm>> SearchAviaSales(FlightSearchParametersDTO request)
    {
        try
        {
            var flightResults = new List<FlightRm>();
            string accessKey = "9dfb38d7c8e19c95700bf9442199fef9";

            int page = 0;

            var fromDate = request.FromDate?.ToString("yyyy-MM-dd");
            var toDate = request.ToDate?.ToString("yyyy-MM-dd");
            string from = "";
            string destination = "";

            if (request.From == "Kutaisi") from = "KUT";
            if (request.Destination == "Memmingen") destination = "FMM";

            string url = $@"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={from}&destination={destination}&departure_at={fromDate}&return_at={toDate}&unique=false&sorting=price&direct=false&cy=usd&limit=30&page=1&one_way=true&token=9dfb38d7c8e19c95700bf9442199fef9";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response); // Assuming you create a class ApiResponse that matches the structure

                // Check if the response contains data
                foreach (var flight in apiResponse.Data)
                {
                    var flightRm = new FlightRm(
                        Guid.NewGuid(), // Generate a new GUID or adapt according to your requirements
                        flight.Airline.Name,
                        "100",
                        //flight.Price.ToString(), // Assuming price is available, adapt as needed
                        new TimePlaceRm(flight.Departure.Airport, flight.Departure.Estimated),
                        new TimePlaceRm(flight.Arrival.Airport, flight.Arrival.Estimated),
                        flight.RemainingNumberOfSeats // Assuming this field exists, adapt as needed
                    );
                    flightResults.Add(flightRm);
                }

                page++;
            }
            return flightResults;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }
    }

    public async Task<IEnumerable<FlightRm>> SearchTEST(FlightSearchParametersDTO @params)
    {
        var flightResults = new List<FlightRm>();
        string accessKey = _aviationStackSettings.AccessKey; // Your new access key
        //string[] airlines = { "Wizz Air", "Pegasus", "LOT", "Qatar Airways", "Turkish Airlines", "American Airlines", "Lufthansa" };
        string[] airlines = { "Wizz Air" };

        foreach (var airline in airlines)
        {
            int page = 0;
            bool hasMoreData = true;

            //while (hasMoreData)
            {
                // Build the API request URL with pagination
                string url = $"https://api.aviationstack.com/v1/flights?access_key={accessKey}&airline={airline}&flight_status=active&page={page}&limit=1";

                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response); // Assuming you create a class ApiResponse that matches the structure

                    // Check if the response contains data
                    if (apiResponse != null && apiResponse.Data != null)
                    {
                        foreach (var flight in apiResponse.Data)
                        {
                            var flightRm = new FlightRm(
                                Guid.NewGuid(), // Generate a new GUID or adapt according to your requirements
                                flight.Airline.Name,
                                "100",
                                //flight.Price.ToString(), // Assuming price is available, adapt as needed
                                new TimePlaceRm(flight.Departure.Airport, flight.Departure.Estimated),
                                new TimePlaceRm(flight.Arrival.Airport, flight.Arrival.Estimated),
                                flight.RemainingNumberOfSeats // Assuming this field exists, adapt as needed
                            );
                            flightResults.Add(flightRm);
                        }

                        hasMoreData = apiResponse.Pagination.Count > 0; // Continue if more data exists
                        page++;
                    }
                    else
                    {
                        hasMoreData = false; // Stop if no data is returned
                    }
                }
            }
        }

        return flightResults;
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

            if (currentMonth == m1 || currentMonth == m2 || currentMonth == m3)
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