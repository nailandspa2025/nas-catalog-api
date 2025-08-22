using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ServicePackages.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ServicePackages.Commands.CreateServicePackage;

public record CreateServicePackageCommand: IRequest<ApiResponse<ServicePackageDto>>
{
    public string Name { get; init; } = null!;

    public bool IsActive { get; init; }

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public int DurationDays { get; init; }
    public List<int> ServiceIds { get; init; } = new List<int>();
    public CurrencyCode Currency { get; init; } = CurrencyCode.USD;
}

public class CreateServicePackageCommandHandler : IRequestHandler<CreateServicePackageCommand, ApiResponse<ServicePackageDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public CreateServicePackageCommandHandler(ICatalogDbContext context, IStorageService storageService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }
    public async Task<ApiResponse<ServicePackageDto>> Handle(CreateServicePackageCommand request, CancellationToken cancellationToken)
    {
        var serviceList = await _context.Service.Where(x => request.ServiceIds.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
        var entity = new ServicePackage
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            Price = request.Price,
            DurationDays = request.DurationDays,
            Currency = request.Currency
        };
        entity.SetServices(serviceList);
        _context.ServicePackage.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ServicePackageDto>.Success(_mapper.Map<ServicePackageDto>(entity));
    }
}
