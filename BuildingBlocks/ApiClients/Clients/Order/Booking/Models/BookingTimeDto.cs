namespace BuildingBlocks.ApiClients.Clients.Order.Booking.Models;

public class BookingTimeDto
{
    public TimeSpan BookingTime { get; set; }
    public DateTime BookingDate { get; set; }
    public List<int> ServiceIds { get; set; } = new List<int>();
}

