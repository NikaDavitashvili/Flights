using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Services;
public interface IFlightService
{
    Task<IEnumerable<FlightRm>> Search(FlightSearchParametersDTO @params);
    Task<FlightRm> Find(Guid id);
    Task Book(BookDTO dto);
}
