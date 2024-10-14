using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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

            //return await _flightRepository.Search(@params) ?? new List<FlightRm>();

            var dic = new Dictionary<string, object>
        {
            { "Destination", @params.Destination ?? string.Empty },
            { "From", @params.From ?? string.Empty },
            { "FromDate", @params.FromDate.HasValue ? @params.FromDate.Value.Date : (object)DBNull.Value },
            { "ToDate", @params.ToDate.HasValue ? @params.ToDate.Value.Date.AddDays(1).AddTicks(-1) : (object)DBNull.Value },
            { "NumberOfPassengers", @params.NumberOfPassengers ?? 1 }
        };

            var query = @"
            SELECT Id, Airline, Price, Departure_Place, Departure_Time, Arrival_Place, Arrival_Time, RemainingNumberOfSeats
            FROM Flights
            WHERE 
                (@Destination = '' OR Arrival_Place LIKE '%' + @Destination + '%') AND
                (@From = '' OR Departure_Place LIKE '%' + @From + '%') AND
                (@FromDate IS NULL OR Departure_Time >= @FromDate) AND
                (@ToDate IS NULL OR Departure_Time <= @ToDate) AND
                RemainingNumberOfSeats >= @NumberOfPassengers
            ORDER BY Departure_Time asc";

            DataTable dt = DB.Select(query, dic, out string errorMessage);

            if (errorMessage != null)
                throw new Exception(errorMessage);

            var flights = new List<FlightRm>();

            foreach (DataRow row in dt.Rows)
            {
                var flight = new FlightRm(
                    Guid.Parse(row["Id"].ToString()),
                    row["Airline"].ToString(),
                    row["Price"].ToString(),
                    "Link",
                    new TimePlaceRm(
                        row["Departure_Place"].ToString(),
                        DateTime.Parse(row["Departure_Time"].ToString())
                    ),
                    new TimePlaceRm(
                        row["Arrival_Place"].ToString(),
                        DateTime.Parse(row["Arrival_Time"].ToString())
                    ),
                    int.Parse(row["RemainingNumberOfSeats"].ToString()),
                    0
                );

                flights.Add(flight);
        }

            return flights;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }
    }


    public async Task<IEnumerable<FlightRm>> SearchAviaSales(FlightSearchParametersDTO request)
    {
        var flightResults = new List<FlightRm>();
        try
        {
            string accessKey = "9dfb38d7c8e19c95700bf9442199fef9";

            var fromDate = request.FromDate?.ToString("yyyy-MM-dd");
            var toDate = request.ToDate?.ToString("yyyy-MM-dd");

            var fromList = new List<string>();
            var toList = new List<string>();

            if (!string.IsNullOrEmpty(request.From)) fromList = await _flightRepository.GetAirportIataCodes(request.From);
            if (!string.IsNullOrEmpty(request.Destination)) toList = await _flightRepository.GetAirportIataCodes(request.Destination);

            if (fromList.Count > 0 && toList.Count > 0)
            {
                foreach (var from in fromList)
                    foreach (var to in toList)
                    {
                        string apiUrl = $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={from}&destination={to}&departure_at={fromDate}&return_at={toDate}&unique=false&sorting=price&direct=false&currency=usd&limit=30&page=1&one_way=false&token={accessKey}";

                        var client = new HttpClient();
                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var flightsResponse = JsonConvert.DeserializeObject<ApiFlightResponse>(content);

                            foreach (var flight in flightsResponse.Data)
                            {
                                var flightRm = new FlightRm(
                                    Guid.NewGuid(),
                                    flight.Airline,
                                    "https://www.aviasales.com" + flight.Link,
                                    flight.Price.ToString(),
                                    new TimePlaceRm(flight.Origin, DateTime.Parse(flight.departure_at)),
                                    new TimePlaceRm(flight.Destination, DateTime.Parse(flight.return_at)),
                                    Convert.ToInt32(flight.FlightNumber),
                                    0
                                );
                                flightResults.Add(flightRm);
                            }
                        }
                    }
            }

            else if (fromList.Count > 0 && toList.Count <= 0)
            {
                foreach (var from in fromList)
                {
                    string apiUrl = $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={from}&departure_at={fromDate}&return_at={toDate}&unique=false&sorting=price&direct=false&currency=usd&limit=30&page=1&one_way=true&token={accessKey}";

                    var client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var flightsResponse = JsonConvert.DeserializeObject<ApiFlightResponse>(content);

                        foreach (var flight in flightsResponse.Data)
                        {
                            var flightRm = new FlightRm(
                                Guid.NewGuid(),
                                flight.Airline,
                                "https://www.aviasales.com" + flight.Link,
                                flight.Price.ToString(),
                                new TimePlaceRm(flight.Origin, DateTime.Parse(flight.departure_at)),
                                new TimePlaceRm(flight.Destination, DateTime.Parse(flight.return_at)),
                                Convert.ToInt32(flight.FlightNumber),
                                0
                            );
                            flightResults.Add(flightRm);
                        }
                    }
                }
            }

            else if (fromList.Count <= 0 && toList.Count > 0)
            {
                foreach (var to in toList)
                {
                    string apiUrl = $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?destination={to}&departure_at={fromDate}&return_at={toDate}&unique=false&sorting=price&direct=false&currency=usd&limit=30&page=1&one_way=true&token={accessKey}";

                    var client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var flightsResponse = JsonConvert.DeserializeObject<ApiFlightResponse>(content);

                        foreach (var flight in flightsResponse.Data)
                        {
                            var flightRm = new FlightRm(
                                Guid.NewGuid(),
                                flight.Airline,
                                "https://www.aviasales.com" + flight.Link,
                                flight.Price.ToString(),
                                new TimePlaceRm(flight.Origin, DateTime.Parse(flight.departure_at)),
                                new TimePlaceRm(flight.Destination, DateTime.Parse(flight.return_at)),
                                Convert.ToInt32(flight.FlightNumber),
                                0
                            );
                            flightResults.Add(flightRm);
                        }
                    }
                }
            }

            return flightResults;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while searching for flights: {ex.Message}");
        }
    }

    //public async Task<IEnumerable<FlightRm>> SearchTEST(FlightSearchParametersDTO @params)
    //{
    //    var flightResults = new List<FlightRm>();
    //    string accessKey = _aviationStackSettings.AccessKey; // Your new access key
    //    //string[] airlines = { "Wizz Air", "Pegasus", "LOT", "Qatar Airways", "Turkish Airlines", "American Airlines", "Lufthansa" };
    //    string[] airlines = { "Wizz Air" };

    //    foreach (var airline in airlines)
    //    {
    //        int page = 0;
    //        bool hasMoreData = true;

    //        //while (hasMoreData)
    //        {
    //            // Build the API request URL with pagination
    //            string url = $"https://api.aviationstack.com/v1/flights?access_key={accessKey}&airline={airline}&flight_status=active&page={page}&limit=1";

    //            using (var client = new HttpClient())
    //            {
    //                var response = await client.GetStringAsync(url);
    //                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response); // Assuming you create a class ApiResponse that matches the structure

    //                // Check if the response contains data
    //                if (apiResponse != null && apiResponse.Data != null)
    //                {
    //                    foreach (var flight in apiResponse.Data)
    //                    {
    //                        var flightRm = new FlightRm(
    //                            Guid.NewGuid(), // Generate a new GUID or adapt according to your requirements
    //                            flight.Airline.Name,
    //                            "100",
    //                            //flight.Price.ToString(), // Assuming price is available, adapt as needed
    //                            new TimePlaceRm(flight.Departure.Airport, flight.Departure.Estimated),
    //                            new TimePlaceRm(flight.Arrival.Airport, flight.Arrival.Estimated),
    //                            flight.RemainingNumberOfSeats // Assuming this field exists, adapt as needed
    //                        );
    //                        flightResults.Add(flightRm);
    //                    }

    //                    hasMoreData = apiResponse.Pagination.Count > 0; // Continue if more data exists
    //                    page++;
    //                }
    //                else
    //                {
    //                    hasMoreData = false; // Stop if no data is returned
    //                }
    //            }
    //        }
    //    }

    //    return flightResults;
    //}

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
    //public async Task<FlightRm> Find(Guid id)
    {
        try
        {
            return await _flightRepository.Find(email);
            //return await _flightRepository.Find(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while finding the flight: {ex.Message}");
        }
    }

    public async Task<string> Book(BookDTO dto, FlightRm flight)
    //public async Task<string> Book(BookDTO dto)
    {
        try
        {
            string result = await _flightRepository.Book(dto, flight);
            //string result = await _flightRepository.Book(dto);

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
    public bool Success { get; set; }
    public List<FlightData> Data { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
    public int Total { get; set; }
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
    public string Origin { get; set; }
    public string Destination { get; set; }
    public int Price { get; set; }
    public string Airline { get; set; }
    public string FlightNumber { get; set; }
    public string departure_at { get; set; }
    public string return_at { get; set; }
    public int Transfers { get; set; }
    public string Link { get; set; }
}


public class ApiFlightResponse
{
    public bool Success { get; set; }
    public List<FlightData> Data { get; set; }
}

public class FlightData
{
    public string Origin { get; set; }
    public string Destination { get; set; }
    public int Price { get; set; }
    public string Airline { get; set; }
    public string FlightNumber { get; set; }
    public string departure_at { get; set; }
    public string return_at { get; set; }
    public int Transfers { get; set; }
    public string Link { get; set; }
}