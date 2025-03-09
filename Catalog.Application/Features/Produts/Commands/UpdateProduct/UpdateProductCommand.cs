using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Produts.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Produts.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public long Id { get; init; }

    public string ProductName { get; init; } = null!;

    public decimal Price { get; init; }

    public string? Description { get; init; }

    public long StoreId { get; init; } 
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProductDto>.Success(_mapper.Map<ProductDto>(entity));
        
    }
}