﻿using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using System.Data;

namespace FinalProject.Infrastructure.Repositories;
public class MapRepository : IMapRepository
{
    public async Task<IEnumerable<CitiesRm>> GetCities()
    {
        var dic = new Dictionary<string, object>() { };

        var query = @"SELECT Departure_Place, Arrival_Place, Price FROM Flights";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        var cities = new List<CitiesRm>();

        foreach (DataRow row in dt.Rows)
        {
            var city = new CitiesRm(
                row["Departure_Place"].ToString(),
                row["Arrival_Place"].ToString(),
                Convert.ToInt32(row["Price"].ToString())
            );

            cities.Add(city);
        }

        return cities;
    }

    public async Task<IEnumerable<OptimalFlightNodeRm>> GetOptimalTripRoute(string departureCity, string arrivalCity)
    {
        var dic = new Dictionary<string, object>()
        {
            { "DepartureCity", departureCity },
            { "ArrivalCity", arrivalCity }
        };

        var query = @"SELECT * FROM Flights
                      WHERE [Departure_Place] <> @ArrivalCity AND [Arrival_Place] <> @DepartureCity";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        var flights = new List<OptimalFlightNodeRm>();

        if (dt == null || dt.Rows.Count == 0)
            return flights;

        foreach (DataRow row in dt.Rows)
        {
            var flight = new OptimalFlightNodeRm(
                Convert.ToInt32(row["Price"].ToString()),
                row["Departure_Place"].ToString(),
                DateTime.Parse(row["Departure_Time"].ToString()),
                row["Arrival_Place"].ToString(),
                DateTime.Parse(row["Arrival_Time"].ToString())
            );

            flights.Add(flight);
        }

        return flights;
    }
}
