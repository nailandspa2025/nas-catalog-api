using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Categories.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand: IRequest<ApiResponse<CategoryDto>>
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public bool IsActive { get; init; }

    public int OrderNo { get; init; }

    public IFormFile? Image { get; init; }

    public int? ParentId { get; init; }

    public bool IsImage { get; init; }

    public List<int> ServiceIds { get; init; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public UpdateCategoryCommandHandler(
        ICatalogDbContext context,
        IMapper mapper,
        IStorageService storageService

        )
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var services = await _context.Service
               .Where(s => request.ServiceIds.Contains(s.Id))
               .ToListAsync(cancellationToken);

        var entity = await _context.Category
           .Include(x => x.Children)
           .Include(x => x.Services)
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;
        entity.OrderNo = request.OrderNo;
        entity.ParentId = request.ParentId;
        entity.SetService(services);
        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == request.Id)
                return ApiResponse<CategoryDto>.Error("Category cannot be its own parent.");

            var parent = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (parent == null)
                return ApiResponse<CategoryDto>.Error("Parent category not found.");

            entity.ParentId = request.ParentId; 
        }

        if (request.Image != null && request.Image.Length > 0)
        {
            var imageUrl = await _storageService.SaveFileAsync(request.Image, cancellationToken);
            if (!string.IsNullOrEmpty(entity.ImageUrl))
                await _storageService.DeleteFileAsync(entity.ImageUrl, cancellationToken);
            entity.ImageUrl = imageUrl;
        }
        else if (request.IsImage)
        {
            if (!string.IsNullOrEmpty(entity.ImageUrl))
                await _storageService.DeleteFileAsync(entity.ImageUrl, cancellationToken);
            entity.ImageUrl = string.Empty;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.Success(_mapper.Map<CategoryDto>(entity));
    }
}
