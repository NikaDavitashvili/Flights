using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using Newtonsoft.Json;
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
                b.FlightId,
                b.NumberOfSeats,
                b.PassengerEmail
            FROM Booking b
            WHERE b.PassengerEmail = @PassengerEmail";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            return new List<BookingRm>();

        var bookings = new List<BookingRm>();

        foreach (DataRow row in dt.Rows)
        {
            var flight = JsonConvert.DeserializeObject<FlightRm>(row["FlightId"].ToString()!);
            var booking = new BookingRm(
                flight!.Id,
                flight.Airline,
                flight.Price,
                flight.Arrival,
                flight.Departure,
                int.Parse(row["NumberOfSeats"].ToString()!),
                row["PassengerEmail"].ToString()!
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
        BEGIN
            DELETE FROM Booking
            WHERE FlightId = @FlightId
              AND PassengerEmail = @PassengerEmail
        END";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);
    }
}
