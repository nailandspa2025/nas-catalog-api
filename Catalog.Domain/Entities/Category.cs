using System;
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class Category: BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

    public virtual Category? Parent { get; set; }

    public virtual ICollection<Category>? Children { get; set; }

    public int OrderNo { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }
}

