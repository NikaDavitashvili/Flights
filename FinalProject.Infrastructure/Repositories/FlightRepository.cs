﻿using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using Newtonsoft.Json;
using System.Data;

namespace FinalProject.Infrastructure.Repositories;
public class FlightRepository : IFlightRepository
{
    public async Task<IEnumerable<FlightRm>?> Search(FlightSearchParametersDTO @params)
    {
        var flights = new List<FlightRm>();
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "flightResults.json");
        if (File.Exists(jsonFilePath))
        {
            var jsonData = await File.ReadAllTextAsync(jsonFilePath);
            flights = JsonConvert.DeserializeObject<List<FlightRm>>(jsonData);
        }

        return flights;
    }

    public async Task<IEnumerable<FlightRm>> SearchByCurrentSeason(int m1, int m2, int m3, int currentYear)
    {
        var flights = new List<FlightRm>();
        var result = new List<FlightRm>();
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "flightResults.json");
        if (File.Exists(jsonFilePath))
        {
            var jsonData = await File.ReadAllTextAsync(jsonFilePath);
            flights = JsonConvert.DeserializeObject<List<FlightRm>>(jsonData);
        }

        foreach(var flight in flights!)
            if(flight.Departure.Time.Year == currentYear)
                if(flight.Departure.Time.Month == m1 || flight.Departure.Time.Month == m2 || flight.Departure.Time.Month == m3)
                    result.Add(flight);

        return result;
    }

    public async Task<IEnumerable<FlightRm>> SearchBySeason(int m1, int m2, int m3)
    {
        var flights = new List<FlightRm>();
        var result = new List<FlightRm>();
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "flightResults.json");
        if (File.Exists(jsonFilePath))
        {
            var jsonData = await File.ReadAllTextAsync(jsonFilePath);
            flights = JsonConvert.DeserializeObject<List<FlightRm>>(jsonData);
        }

        foreach (var flight in flights!)
                if (flight.Departure.Time.Month == m1 || flight.Departure.Time.Month == m2 || flight.Departure.Time.Month == m3)
                    result.Add(flight);

        return result;
    }

    public async Task<List<FlightRm>> Find(string email)
    {
        var dic = new Dictionary<string, object> { { "PassengerEmail", email } };

        var query = @"
            SELECT Id, Airline, Price, Departure_Place, Departure_Time, Arrival_Place, Arrival_Time, RemainingNumberOfSeats
            FROM Flights WHERE Id = @Id";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        var row = dt.Rows[0];

        var flight = new FlightRm(
            Guid.Parse(row["Id"].ToString()),
            row["Airline"].ToString(),
            row["Price"].ToString(),
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

        return new List<FlightRm>();
    }

    public async Task<string> Book(BookDTO dto, FlightRm flight)
    {
        flight = flight with { SeatsToBuy = 0 };
        var s = JsonConvert.SerializeObject(flight);

        var dic = new Dictionary<string, object>
        {
            //{ "FlightId", $"{dto.PassengerEmail}-{flight.Price}-{JsonConvert.SerializeObject(flight.Departure)}-{JsonConvert.SerializeObject(flight.Arrival)}-{flight.Airline}"},
            { "FlightId", JsonConvert.SerializeObject(flight) },
            { "PassengerEmail", dto.PassengerEmail },
            { "NumberOfSeats", dto.NumberOfSeats }
        };

        var query = @"
        DECLARE @BookingId INT;
        DECLARE @ExistingSeats INT;
        
        BEGIN
            IF @NumberOfSeats > 10
            BEGIN
                THROW 50002, 'Cannot add more than 10 seats in a single booking.', 1;
            END
        
            -- Check if the booking already exists
            SELECT @BookingId = Id, @ExistingSeats = NumberOfSeats
            FROM Booking
            WHERE FlightId = @FlightId AND PassengerEmail = @PassengerEmail;
        
            IF @BookingId IS NOT NULL
            BEGIN
                DECLARE @TotalSeats INT;
                SET @TotalSeats = @ExistingSeats + @NumberOfSeats;
        
                IF @TotalSeats > 10
                BEGIN
                    THROW 50003, 'Cannot add more than 10 seats for this booking.', 1;
                END
        
                -- Update the existing booking
                UPDATE Booking
                SET NumberOfSeats = @TotalSeats
                WHERE Id = @BookingId;  -- Use the BookingId to update the specific booking
            END
            ELSE
            BEGIN
                -- Insert new booking if it doesn't exist
                INSERT INTO Booking (FlightId, PassengerEmail, NumberOfSeats)
                VALUES (@FlightId, @PassengerEmail, @NumberOfSeats);
            END
        END";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            return "One user can't book more than 10 tickets on one flight!";

        if (dt != null && dt.Rows.Count != 0)
            return dt.Rows[0].ItemArray[0].ToString();

        return "Ok";
    }

    /* public async Task<IEnumerable<FlightRm>> Search(FlightSearchParametersDTO @params)
    {
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
                new TimePlaceRm(
                    row["Departure_Place"].ToString(),
                    DateTime.Parse(row["Departure_Time"].ToString())
                ),
                new TimePlaceRm(
                    row["Arrival_Place"].ToString(),
                    DateTime.Parse(row["Arrival_Time"].ToString())
                ),
                int.Parse(row["RemainingNumberOfSeats"].ToString())
            );

            flights.Add(flight);
        }

        return flights;
    } 

    public async Task<IEnumerable<FlightRm>> SearchByCurrentSeason(int m1, int m2, int m3, int currentYear)
    {
        var dic = new Dictionary<string, object>
        {
            { "Month1", m1 },
            { "Month2", m2 },
            { "Month3", m3 },
            { "CurrentYear", currentYear }
        };

        var query = @"
            SELECT TOP(5) Id, Airline, Price, Departure_Place, Departure_Time, Arrival_Place, Arrival_Time, RemainingNumberOfSeats
            FROM Flights
            WHERE YEAR(Departure_Time) = @CurrentYear AND MONTH(Departure_Time) IN (@Month1, @Month2, @Month3)
            ORDER BY RemainingNumberOfSeats DESC, Departure_Time ASC";

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
                new TimePlaceRm(
                    row["Departure_Place"].ToString(),
                    DateTime.Parse(row["Departure_Time"].ToString())
                ),
                new TimePlaceRm(
                    row["Arrival_Place"].ToString(),
                    DateTime.Parse(row["Arrival_Time"].ToString())
                ),
                int.Parse(row["RemainingNumberOfSeats"].ToString())
            );

            flights.Add(flight);
        }

        return flights;
    }

    public async Task<IEnumerable<FlightRm>> SearchBySeason(int m1, int m2, int m3)
    {
        var dic = new Dictionary<string, object>
        {
            { "Month1", m1 },
            { "Month2", m2 },
            { "Month3", m3 }
        };

        var query = @"
            SELECT TOP(5) Id, Airline, Price, Departure_Place, Departure_Time, Arrival_Place, Arrival_Time, RemainingNumberOfSeats
            FROM Flights
            WHERE MONTH(Departure_Time) IN (@Month1, @Month2, @Month3)
            ORDER BY NEWID()";

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
                new TimePlaceRm(
                    row["Departure_Place"].ToString(),
                    DateTime.Parse(row["Departure_Time"].ToString())
                ),
                new TimePlaceRm(
                    row["Arrival_Place"].ToString(),
                    DateTime.Parse(row["Arrival_Time"].ToString())
                ),
                int.Parse(row["RemainingNumberOfSeats"].ToString())
            );

            flights.Add(flight);
        }

        return flights;
    }

    public async Task<FlightRm> Find(Guid id)
    {
        var dic = new Dictionary<string, object> { { "Id", id } };

        var query = @"
            SELECT Id, Airline, Price, Departure_Place, Departure_Time, Arrival_Place, Arrival_Time, RemainingNumberOfSeats
            FROM Flights WHERE Id = @Id";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        var row = dt.Rows[0];

        var flight = new FlightRm(
            Guid.Parse(row["Id"].ToString()),
            row["Airline"].ToString(),
            row["Price"].ToString(),
            new TimePlaceRm(
                row["Departure_Place"].ToString(),
                DateTime.Parse(row["Departure_Time"].ToString())
            ),
            new TimePlaceRm(
                row["Arrival_Place"].ToString(),
                DateTime.Parse(row["Arrival_Time"].ToString())
            ),
            int.Parse(row["RemainingNumberOfSeats"].ToString())
        );

        return flight;
    } */
}
