namespace FinalProject.Domain.Models.DTOs;
public class Flight
{

    public Guid Id { get; set; }
    public string Airline { get; set; }
    public string Price { get; set; }
    public TimePlaceDTO Departure { get; set; }
    public TimePlaceDTO Arrival { get; set; }
    public int RemainingNumberOfSeats { get; set; }

    public IList<Booking> Bookings = new List<Booking>();

    public Flight()
    {

    }

    public Flight(
        Guid id,
        string airline,
        string price,
        TimePlaceDTO departure,
        TimePlaceDTO arrival,
        int remainingNumberOfSeats
    )
    {
        Id = id;
        Airline = airline;
        Price = price;
        Departure = departure;
        Arrival = arrival;
        RemainingNumberOfSeats = remainingNumberOfSeats;
    }


    public object? MakeBooking(string passengerEmail, byte numberOfSeats)
    {
        var flight = this;

        if (flight.RemainingNumberOfSeats < numberOfSeats)
        {
            return new OverbookErrorDTO();
        }

        flight.Bookings.Add(
            new Booking(
                passengerEmail,
                numberOfSeats)
            );

        flight.RemainingNumberOfSeats -= numberOfSeats;
        return null;
    }

    public object? CancelBooking(string passengerEmail, byte numberOfSeats)
    {
        var booking = Bookings.FirstOrDefault(b => numberOfSeats == b.NumberOfSeats
       && passengerEmail.ToLower() == b.PassengerEmail.ToLower());

        if (booking == null)
            return new NotFoundErrorDTO();

        Bookings.Remove(booking);
        RemainingNumberOfSeats += booking.NumberOfSeats;

        return null;
    }
}
