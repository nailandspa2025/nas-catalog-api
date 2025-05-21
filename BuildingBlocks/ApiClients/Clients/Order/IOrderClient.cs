using BuildingBlocks.ApiClients.Clients.Order.Booking.Models;
using Refit;

namespace BuildingBlocks.ApiClients.Clients.Order;

public interface IOrderClient
{
    [Refit.Get("/api/v1/bookings/technician")]
    Task<Core.Response.ApiResponse<IEnumerable<BookingTimeDto>>> GetBookedSlotsAsync(
        [Query] long storeId,
        [Query] long technicianId,
        [Query] DateTime date,
        CancellationToken cancellationToken = default
      );
}