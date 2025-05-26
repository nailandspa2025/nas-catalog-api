using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class ReviewStore : BaseAuditableEntity<int>, ISoftDelete
{
    public int BookingId { get; set; }

    public long StoreId { get; set; }

    public int StoreRating { get; set; }

    public long TechnicianId { get; set; }

    public int TechnicianRating { get; set; }

    public int ServiceId { get; set; }

    public int ServiceRating { get; set; }

    public string? Content { get; set; }

    public bool IsActive { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public Store Store { get; set; }
}

