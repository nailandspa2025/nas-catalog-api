using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Produts.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Produts.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public long Id { get; init; }

    public string ProductName { get; init; } = null!;

    public decimal Price { get; init; }

    public string? Description { get; init; }

    public long? StoreId { get; init; } 
    public IFormFile? ImageUrl { get; init; } 
    public bool IsImage { get; init; }

}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public UpdateProductCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public  async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Product
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if(entity == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }
        //var properties = await _context.Store
        //    .Where(x => request.StoreIds.Contains(x.Id))
        //    .ToListAsync(cancellationToken: cancellationToken);

        entity.ProductName = request.ProductName;
        entity.Price = request.Price;
        entity.Description = request.Description;
        entity.StoreId = request.StoreId;

        if (request.ImageUrl is not null)
        {
            var oldImage = entity.ImageUrl;

            entity.ImageUrl = await _storageService.SaveFileAsync(
                request.ImageUrl,
                cancellationToken
            );

            if (!string.IsNullOrEmpty(oldImage))
                await _storageService.DeleteFileAsync(oldImage, cancellationToken);
        }
        else if (request.IsImage)
        {
            if (!string.IsNullOrEmpty(entity.ImageUrl))
                await _storageService.DeleteFileAsync(entity.ImageUrl, cancellationToken);
            entity.ImageUrl = string.Empty;
        }
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProductDto>.Success(_mapper.Map<ProductDto>(entity));
        
    }
}