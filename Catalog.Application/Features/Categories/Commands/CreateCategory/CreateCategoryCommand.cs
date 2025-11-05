using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Categories.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand: IRequest<ApiResponse<CategoryDto>>
{
	public string Name { get; init; } = null!;

	public string ?  Description { get; init; }

	public bool IsActive { get; init; }

	public int OrderNo { get; init; }

    public IFormFile ? Image { get; init; }

    public int? ParentId { get; init; }

    public List<int> ServiceIds { get; init; }
}


public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public CreateCategoryCommandHandler(
        ICatalogDbContext context,
        IMapper mapper,
        IStorageService storageService

        )
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }
    public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Name = request.Name,
            IsActive = request.IsActive,
            OrderNo = request.OrderNo,
            Description = request.Description
        };
        if(request.ParentId.HasValue)
        {
            var parentCategory = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (parentCategory == null)
            {
                return ApiResponse<CategoryDto>.Error("Parent category not found");
            }
            entity.Parent = parentCategory;
        }
        if (request.ServiceIds != null && request.ServiceIds.Any())
        {
            var services = await _context.Service
                .Where(s => request.ServiceIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            entity.SetService(services);
        }

        if (request.Image != null && request.Image.Length > 0)
        {
            var imageUrl = await _storageService.SaveFileAsync(request.Image, cancellationToken);
            entity.ImageUrl = imageUrl;
        }
        _context.Category.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.Success(_mapper.Map<CategoryDto>(entity));
    }
}
