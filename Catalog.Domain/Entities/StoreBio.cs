using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class StoreBio : BaseAuditableEntity<int> , ISoftDelete
{
    public string ? Text { get; set; }

    public string ? File { get; set; }

    public string ?  Image { get; set;  }

    public long StoreId { get; set; }

    public Store ? Store { get; set; }

    public bool IsActive { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }
}

