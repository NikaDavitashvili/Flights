using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Repositories;
public interface IFlightRepository
{
    IEnumerable<FlightRm> Search(FlightSearchParametersDTO @params);
    FlightRm Find(Guid id);
    void Book(BookDTO dto);
}

