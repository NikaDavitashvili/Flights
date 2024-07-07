using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using System.Data;

namespace FinalProject.Infrastructure.Repositories;
public class BookingRepository : IBookingRepository
{
    public async Task<IEnumerable<BookingRm>> List(string email)
    {
        var dic = new Dictionary<string, object>
        {
            { "PassengerEmail", email }
        };

        var query = @"
            SELECT 
                f.Id AS FlightId,
                f.Airline,
                f.Price,
                f.Departure_Place,
                f.Departure_Time,
                f.Arrival_Place,
                f.Arrival_Time,
                b.NumberOfSeats,
                b.PassengerEmail
            FROM Flights f
            INNER JOIN Booking b ON f.Id = b.FlightId
            WHERE b.PassengerEmail = @PassengerEmail";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        var bookings = new List<BookingRm>();

        foreach (DataRow row in dt.Rows)
        {
            var booking = new BookingRm(
                Guid.Parse(row["FlightId"].ToString()),
                row["Airline"].ToString(),
                row["Price"].ToString(),
                new TimePlaceRm(
                    row["Arrival_Place"].ToString(),
                    DateTime.Parse(row["Arrival_Time"].ToString())
                ),
                new TimePlaceRm(
                    row["Departure_Place"].ToString(),
                    DateTime.Parse(row["Departure_Time"].ToString())
                ),
                int.Parse(row["NumberOfSeats"].ToString()),
                row["PassengerEmail"].ToString()
            );

            bookings.Add(booking);
        }

        return bookings;

    }

    public async Task CancelBooking(BookDTO dto)
    {
        var dic = new Dictionary<string, object>
        {
            { "FlightId", dto.FlightId },
            { "PassengerEmail", dto.PassengerEmail },
            { "NumberOfSeats", dto.NumberOfSeats }
        };

        var query = @"
            DECLARE @RemainingSeats INT;

            -- Find the booking
            SELECT @RemainingSeats = NumberOfSeats
            FROM Booking
            WHERE FlightId = @FlightId AND PassengerEmail = @PassengerEmail AND NumberOfSeats = @NumberOfSeats;

            -- Check if booking exists
            IF @RemainingSeats IS NULL
            BEGIN
                THROW 500, 'Booking not found', 1;
            END

            -- Remove the booking
            DELETE FROM Booking
            WHERE FlightId = @FlightId AND PassengerEmail = @PassengerEmail AND NumberOfSeats = @NumberOfSeats;

            -- Update the remaining number of seats in the flight
            UPDATE Flights
            SET RemainingNumberOfSeats = RemainingNumberOfSeats + @NumberOfSeats
            WHERE Id = @FlightId;";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);
    }
}
