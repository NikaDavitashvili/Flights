using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IBookingRepository
{
    Task<IEnumerable<BookingRm>> List(string email);
    Task CancelBooking(BookDTO dto);
}
