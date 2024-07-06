using System.Text.Json.Nodes;

namespace FinalProject.Domain.Common;
public class Configuration
{
    public static JsonNode Data = JsonNode.Parse(File.ReadAllText(@"appsettings.json")) ?? throw new Exception("Error! Could not parse Configuration File!");

    public static readonly ConnectionString ConnectionStrings = new ConnectionString();
}
public class ConnectionString
{
    public string Flights = Configuration.Data["ConnectionStrings"]?["Flights"]?.ToString() ?? throw new Exception("Error! Configuration empty ConnectionString 'Flights'!");
}
