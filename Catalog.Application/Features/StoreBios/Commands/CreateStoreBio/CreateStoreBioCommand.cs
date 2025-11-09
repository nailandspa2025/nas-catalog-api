using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.StoreBios.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.StoreBios.Commands.CreateStoreBio;

public record CreateStoreBioCommand: IRequest<ApiResponse<StoreBioDto>>
{
	public string ? Text { get; init; }

	public IFormFile ? File { get; init; }

	public IFormFile ? Image { get; init; }

	public long StoreId { get; init; }

    public bool IsActive { get; init; }
}

public class CreateStoreBioCommandHandler : IRequestHandler<CreateStoreBioCommand, ApiResponse<StoreBioDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public CreateStoreBioCommandHandler(ICatalogDbContext context, IStorageService storageService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }
    public async Task<ApiResponse<StoreBioDto>> Handle(CreateStoreBioCommand request, CancellationToken cancellationToken)
    {
        if (await _context.StoreBio.AsNoTracking().AnyAsync(x => x.StoreId == request.StoreId, cancellationToken))
        {
            return ApiResponse<StoreBioDto>.Error("Store already has a bio. Use update instead.");
        }

        var entity = new StoreBio
        {
            Text = request.Text,
            StoreId = request.StoreId,
            IsActive = request.IsActive
        };

        if (request.Image != null && request.Image.Length > 0)
        {
            var imagePath = await _storageService.SaveFileAsync(request.Image, cancellationToken);
            entity.Image = imagePath;
        }

        if (request.File != null && request.File.Length > 0)
        {
            var filePath = await _storageService.SaveFileAsync(request.File, cancellationToken);

            entity.File = filePath; 
        }
        _context.StoreBio.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<StoreBioDto>.Success(_mapper.Map<StoreBioDto>(entity));
    }
}