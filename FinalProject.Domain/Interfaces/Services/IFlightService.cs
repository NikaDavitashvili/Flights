﻿using FinalProject.Domain.Models.DTOs;
using FinalProject.Domain.Models.ReadModels;

namespace FinalProject.Domain.Interfaces.Services;
public interface IFlightService
{
    Task<IEnumerable<FlightRm>> Search(FlightSearchParametersDTO @params);
    Task<IEnumerable<FlightRm>> SearchBySeason(string seasonName);
    Task<List<FlightRm>> Find(string email);
    Task<string> Book(BookDTO dto, FlightRm flight);
}