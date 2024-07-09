using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using System.Data;

namespace FinalProject.Infrastructure.Repositories;
public class FlightRepository : IFlightRepository
{
    public async Task<IEnumerable<FlightRm>> Search(FlightSearchParametersDTO @params)
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
            SELECT 
                Id,
                Airline,
                Price,
                Departure_Place,
                Departure_Time,
                Arrival_Place,
                Arrival_Time,
                RemainingNumberOfSeats
            FROM Flights
            WHERE 
                (@Destination = '' OR Arrival_Place LIKE '%' + @Destination + '%') AND
                (@From = '' OR Departure_Place LIKE '%' + @From + '%') AND
                (@FromDate IS NULL OR Departure_Time >= @FromDate) AND
                (@ToDate IS NULL OR Departure_Time <= @ToDate) AND
                RemainingNumberOfSeats >= @NumberOfPassengers";

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
            SELECT 
                Id,
                Airline,
                Price,
                Departure_Place,
                Departure_Time,
                Arrival_Place,
                Arrival_Time,
                RemainingNumberOfSeats
            FROM Flights
            WHERE Id = @Id";

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
    }
    public async Task<string> Book(BookDTO dto)
    {
        var dic = new Dictionary<string, object>
        {
            { "FlightId", dto.FlightId },
            { "PassengerEmail", dto.PassengerEmail },
            { "NumberOfSeats", dto.NumberOfSeats }
        };

        var query = @"
        DECLARE @BookingId INT;
        DECLARE @CurrentRemainingSeats INT;
        
        SELECT @CurrentRemainingSeats = RemainingNumberOfSeats
        FROM Flights
        WHERE Id = @FlightId;
        
        IF @CurrentRemainingSeats < @NumberOfSeats
        BEGIN
            THROW 50003, 'Not enough seats available for this booking.', 1;
        END
        
        SELECT @BookingId = Id
        FROM Booking
        WHERE FlightId = @FlightId AND PassengerEmail = @PassengerEmail;
        
        IF @BookingId IS NOT NULL
        BEGIN
            DECLARE @TotalSeats INT;
            SELECT @TotalSeats = NumberOfSeats + @NumberOfSeats
            FROM Booking
            WHERE Id = @BookingId;
        
            IF @TotalSeats > 10
            BEGIN
                THROW 500, 'Cannot add more than 10 seats for this booking.', 1;
            END
        
            UPDATE Booking
            SET NumberOfSeats = @TotalSeats
            WHERE Id = @BookingId;
        END
        ELSE
        BEGIN
            IF @NumberOfSeats > 10
            BEGIN
                THROW 50002, 'Cannot add more than 10 seats in a single booking.', 1;
            END
        
            INSERT INTO Booking (FlightId, PassengerEmail, NumberOfSeats)
            VALUES (@FlightId, @PassengerEmail, @NumberOfSeats);
        END
        
        UPDATE Flights
        SET RemainingNumberOfSeats = RemainingNumberOfSeats - @NumberOfSeats
        WHERE Id = @FlightId;";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            return "One user can't book more than 10 tickets on one flight!";

        if (dt != null && dt.Rows.Count != 0)
            return dt.Rows[0].ItemArray[0].ToString();

        return "Ok";
    }
}
