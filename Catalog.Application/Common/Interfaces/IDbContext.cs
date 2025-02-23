
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Common.Interfaces;


public interface IDbContext: IEfCoreDbContext
{
    //DbSet<Catalog.Domain.Entities.Booking> Booking { get; }
}

