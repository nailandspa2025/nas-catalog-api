using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Services.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Services.Commands.UpdateService;
public record UpdateServiceCommand : IRequest<ApiResponse<ServiceDto>>
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? Code { get; init; }
    public IFormFile? Image { get; init; }
    public bool IsImage { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public int? Rating { get; init; }
    public TimeSpan? WorkingTime { get; init; }
    public CurrencyCode Currency { get; init; }
    public double Commission { get; init; } 
    
}

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ApiResponse<ServiceDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public UpdateServiceCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public async Task<ApiResponse<ServiceDto>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Service.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Code = request.Code;
        entity.Rating = request.Rating;
        entity.PriceFrom = request.PriceFrom;
        entity.PriceTo = request.PriceTo;
        entity.WorkingTime = request.WorkingTime;
        entity.Currency = request.Currency;
        entity.Commission = request.Commission;
        if (request.Image != null)
        {
            var url = await _storageService.SaveFileAsync(request.Image, cancellationToken);
            if (!string.IsNullOrEmpty(entity.UrlImage))
                await _storageService.DeleteFileAsync(entity.UrlImage, cancellationToken);
            entity.UrlImage = url;
        }
        else if (request.IsImage)
        {
            if (!string.IsNullOrEmpty(entity.UrlImage))
                await _storageService.DeleteFileAsync(entity.UrlImage, cancellationToken);
            entity.UrlImage = string.Empty;
        }
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ServiceDto>.Success(_mapper.Map<ServiceDto>(entity));
    }
}
