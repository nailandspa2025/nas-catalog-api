using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Services.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Features.Services.Commands.CreateService;

public record CreateServiceCommand : IRequest<ApiResponse<ServiceDto>>
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? Code { get; init; }
    public IFormFile? Image { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public int? Rating { get; init; }
    public TimeSpan? WorkingTime { get; init; }
    public CurrencyCode Currency { get; init; } = CurrencyCode.USD;
    public double Commission { get; init; } 
    public CommissionType CommissionType { get; init; }
}

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ApiResponse<ServiceDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public CreateServiceCommandHandler (ICatalogDbContext context, IStorageService storageService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }
    public async Task<ApiResponse<ServiceDto>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = new Service
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            PriceFrom = request.PriceFrom,
            PriceTo = request.PriceTo,
            Rating = request.Rating,
            WorkingTime = request.WorkingTime,
            Currency = request.Currency,
            Commission = request.Commission,
            CommissionType = request.CommissionType
        };
        if (request.Image != null)
        { 
            var url = await _storageService.SaveFileAsync(request.Image, cancellationToken);
            entity.UrlImage = url;
        }
        _context.Service.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ServiceDto>.Success(_mapper.Map<ServiceDto>(entity));
    }
}
