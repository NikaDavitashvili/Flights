using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Infrastructure.Repositories;
public class FlightRepository : IFlightRepository
{
    public Task<string> Book(BookDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<FlightRm?> Find(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<FlightRm>> Search(FlightSearchParametersDTO @params)
    {
        throw new NotImplementedException();
    }
}