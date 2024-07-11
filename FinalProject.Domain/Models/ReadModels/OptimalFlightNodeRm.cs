namespace FinalProject.Domain.Models.ReadModels;
public record OptimalFlightNodeRm
(
     int Price,
     string DepartureCity,
     DateTime DepartureTime,
     string ArrivalCity,
     DateTime ArrivalTime
);

public class Node
{
    public string City { get; set; }
    public decimal Price { get; set; }
    public List<OptimalFlightNodeRm> Path { get; set; }

    public Node(string city, decimal price, List<OptimalFlightNodeRm> path)
    {
        City = city;
        Price = price;
        Path = path;
    }
}