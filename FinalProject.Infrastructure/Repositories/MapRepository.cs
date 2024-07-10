using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Domain.Models.ReadModels;
using FinalProject.Infrastructure.Common;
using System.Data;

namespace FinalProject.Infrastructure.Repositories;
public class MapRepository : IMapRepository
{
    public async Task<IEnumerable<CitiesRm>> GetCities()
    {
        var dic = new Dictionary<string, object>() { };

        var query = @"SELECT Departure_Place, Arrival_Place FROM Flights";

        DataTable dt = DB.Select(query, dic, out string errorMessage);

        if (errorMessage != null)
            throw new Exception(errorMessage);

        if (dt == null || dt.Rows.Count == 0)
            throw new Exception("Data Not Found!");

        var cities = new List<CitiesRm>();

        foreach (DataRow row in dt.Rows)
        {
            var city = new CitiesRm(
                row["Departure_Place"].ToString(),
                row["Arrival_Place"].ToString()
            );

            cities.Add(city);
        }

        return cities;
    }
}
