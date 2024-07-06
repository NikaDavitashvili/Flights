using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IFlightRepository
{
    Task<IQueryable<FlightRm>> Search(FlightSearchParametersDTO @params);
    Task<FlightRm?> Find(Guid id);
    Task<string> Book(BookDTO dto);
}
