using System.Text.Json.Nodes;

namespace FinalProject.Domain.Common;
public class Configuration
{
    public static JsonNode Data = JsonNode.Parse(File.ReadAllText(@"appsettings.json")) ?? throw new Exception("Error! Could not parse Configuration File!");

    public static readonly ConnectionString ConnectionStrings = new ConnectionString();
    public static readonly FlightsSchedulerSettings FlightsSchedulerSettings = new FlightsSchedulerSettings();
    public static readonly AviationStackAPISettings AviationStackAPISettings = new AviationStackAPISettings();
}
public class ConnectionString
{
    public string Flights = Configuration.Data["ConnectionStrings"]?["Flights"]?.ToString() ?? throw new Exception("Error! Configuration empty ConnectionString 'Flights'!");
}
public class FlightsSchedulerSettings
{
    public string WorkingIntervalDays = Configuration.Data["FlightsSchedulerSettings"]?["WorkingIntervalDays"]?.ToString() ?? throw new Exception("Error! Configuration empty FlightsSchedulerSettings 'WorkingIntervalDays'!");
}
public class AviationStackAPISettings
{
    public string AccessKey = Configuration.Data["AviationStackAPISettings"]?["AccessKey"]?.ToString() ?? throw new Exception("Error! Configuration empty AviationStackAPISettings 'AccessKey'!");
}
