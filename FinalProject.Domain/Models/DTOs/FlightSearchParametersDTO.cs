﻿using System.ComponentModel;

namespace FinalProject.Domain.Models.DTOs;
public record FlightSearchParametersDTO(
    
    [DefaultValue("12/25/2022 10:30:00 AM")]
    DateTime? FromDate,

    [DefaultValue("12/26/2022 10:30:00 AM")]
    DateTime? ToDate,

    [DefaultValue("Los Angeles")]
    string? From,

    [DefaultValue("Berlin")]
    string? Destination,

    [DefaultValue(1)]
    int? NumberOfPassengers,

    string? SeasonName

    );

