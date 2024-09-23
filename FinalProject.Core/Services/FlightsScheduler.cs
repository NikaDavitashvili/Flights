using FinalProject.Domain.Common;
using FinalProject.Domain.Models.ReadModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace FinalProject.Core.Services;

public class FlightsScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string AkidoAgreementUrl = "/api/Printing/sesx_gamart_forma_akido";
    private readonly string LoanScheduleUrl = "/api/Printing/Index?LoanId={0}&Type={1}&OnlineSignature={2}";

    public FlightsScheduler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task RunSchedulerAsync(CancellationToken cancellationToken)
    {
        await ExecuteAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Georgian Standard Time");
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(now, tz);

            var nextExecution = new DateTime(localTime.Year, localTime.Month, localTime.Day, 4, 0, 0);

            if (localTime >= nextExecution)
            {
                nextExecution = nextExecution.AddDays(1);
            }

            var delay = nextExecution - localTime;

            await Task.Delay(delay, stoppingToken);

            using (var scope = _scopeFactory.CreateScope())
            {
                var airlines = new List<string> { "W6", "LO", "PC", "QR", "TK", "AA", "LH" };
                var flightResults = new List<FlightRm>();

                foreach (var airline in airlines)
                {
                    int page = 0;
                    bool hasMoreData = true;

                    string url = $"https://api.aviationstack.com/v1/flights?access_key={Configuration.AviationStackAPISettings.AccessKey}&airline_iata={airline}&flight_status=active&page={page}&limit=10";

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

                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "flightResults.json");
                //var cacheData = new FlightCache
                //{
                //    CachedTime = DateTime.Now,
                //    FlightResults = flightResults
                //};
                var jsonData = JsonConvert.SerializeObject(flightResults);
                await File.WriteAllTextAsync(jsonFilePath, jsonData);
            }
        }
    }

    public async Task<bool> RunJobItem()
    {
        try
        {
            var airlines = new List<string> { "W6", "LO", "PC", "QR", "TK", "AA", "LH" };
            var flightResults = new List<FlightRm>();

            foreach (var airline in airlines)
            {
                int page = 0;
                bool hasMoreData = true;

                string url = $"https://api.aviationstack.com/v1/flights?access_key={Configuration.AviationStackAPISettings.AccessKey}&airline_iata={airline}&flight_status=active&page={page}&limit=10";

                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response);

                    if (apiResponse != null && apiResponse.Data != null)
                    {
                        foreach (var flight in apiResponse.Data)
                        {
                            var randomPrice = new Random();
                            var flightRm = new FlightRm(
                                Guid.NewGuid(),
                                flight.Airline.Name,
                                (randomPrice.Next(100, 300)).ToString(),
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

            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "flightResults.json");
            //var cacheData = new FlightCache
            //{
            //    CachedTime = DateTime.Now,
            //    FlightResults = flightResults
            //};
            var jsonData = JsonConvert.SerializeObject(flightResults);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }
}

