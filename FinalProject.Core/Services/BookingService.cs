using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.Data;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Core.Services;
public class BookingService : IBookingService
{
    private readonly Entities _entities;
    private readonly IBookingRepository _bookingRepository;

    public BookingService(Entities entities, IBookingRepository bookingRepository)
    {
        _entities = entities;
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingRm>> List(string email)
    {
        var bookings = await _bookingRepository.List(email);

        return bookings;
    }

    public async Task Cancel(BookDTO dto)
    {
        var flight = _entities.Flights.Find(dto.FlightId);

        var error = flight?.CancelBooking(dto.PassengerEmail, dto.NumberOfSeats);

        if (error == null)
        {
            _entities.SaveChanges();
        }

        if (error is NotFoundErrorDTO)
            throw new Exception("ERROR");

        throw new Exception($"The error of type: {error.GetType().Name} occurred while canceling the booking made by {dto.PassengerEmail}");

    }
}
