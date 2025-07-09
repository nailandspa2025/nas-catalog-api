using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class BankAccount : BaseAuditableEntity<int>, ISoftDelete
{
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string BranchName { get; set; } = null!;
    public string? SwiftCode { get; set; }
    public string? CurrencyCode { get; set; }
    public string? DeletedBy { get ; set ; }
    public DateTime? Deleted { get ; set ; }
    public bool IsDeleted { get ; set ; }
     
    public virtual ICollection<Store> Stores { get; set; }  = new List<Store>();
}
