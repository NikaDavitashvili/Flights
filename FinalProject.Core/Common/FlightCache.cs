using FinalProject.Domain.Models.ReadModels;

public class FlightCache
{
    public DateTime CachedTime { get; set; }
    public IEnumerable<FlightRm> FlightResults { get; set; }
}