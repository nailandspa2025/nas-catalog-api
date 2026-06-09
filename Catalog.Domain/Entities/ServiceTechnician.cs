using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class ServiceTechnician : BaseEntity
{
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public int TechnicianId { get; set; }
}