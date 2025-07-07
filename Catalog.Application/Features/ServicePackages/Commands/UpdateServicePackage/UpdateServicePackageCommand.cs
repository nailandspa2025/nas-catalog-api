using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ServicePackages.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ServicePackages.Commands.UpdateServicePackage;

public record UpdateServicePackageCommand : IRequest<ApiResponse<ServicePackageDto>>
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public bool IsActive { get; init; }

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public int DurationDays { get; init; }

    public List<int> ServiceIds { get; init; } = new List<int>();
}


public class UpdateServicePackageCommandHandler : IRequestHandler<UpdateServicePackageCommand, ApiResponse<ServicePackageDto>>
{
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _context;

    public UpdateServicePackageCommandHandler(IMapper mapper, ICatalogDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ServicePackageDto>> Handle(UpdateServicePackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ServicePackage
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(ServicePackage), request.Id);
        }
        var serviceList = await _context.Service.Where(x => request.ServiceIds.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.DurationDays = request.DurationDays;
        entity.IsActive = request.IsActive;
        entity.SetServices(serviceList);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ServicePackageDto>.Success(_mapper.Map<ServicePackageDto>(entity));
    }
}
