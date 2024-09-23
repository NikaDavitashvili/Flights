using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Services;
public interface IBookingService
{
    Task<IEnumerable<BookingRm>> List(string email);
    Task Cancel(BookDTO dto);
}
