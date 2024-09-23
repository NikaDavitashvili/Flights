using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Core.Services;
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingRm>> List(string email)
    {
        var bookings = await _bookingRepository.List(email);

        return bookings;
    }

    public async Task Cancel(BookDTO dto)
    {
        try
        {
            await _bookingRepository.CancelBooking(dto);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Booking not found"))
                throw new Exception("ERROR");

            throw new Exception($"The error of type: {ex.GetType().Name} occurred while canceling the booking made by {dto.PassengerEmail}");
        }
    }
}
