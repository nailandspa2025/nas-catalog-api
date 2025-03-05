using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Produts.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Produts.Commands.CreateProduct;

public record CreateProductCommand: IRequest<ApiResponse<ProductDto>>
{
    public string ProductName { get; init; } = null!;

    public decimal Price { get; init; }

    public string? Description { get; init; }

    public List<long> StoreIds { get; init; } = new List<long>();
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    
    public CreateProductCommandHandler (ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var properties = await _context.Store
            .Where(x => request.StoreIds.Contains(x.Id))
            .ToListAsync(cancellationToken: cancellationToken);

        var entity = new Product
        {
            ProductName = request.ProductName,
            Price = request.Price,
            Description = request.Description,
           
        };
        entity.SetStores(properties);
        _context.Product.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProductDto>.Success(_mapper.Map<ProductDto>(entity));
 
    }
}