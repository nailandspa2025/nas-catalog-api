using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.StoreBios.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.StoreBios.Commands.UpdateStoreBio;

public class UpdateStoreBioCommand: IRequest<ApiResponse<StoreBioDto>>
{
    public int Id { get; init; }

    public string? Text { get; init; }

    public IFormFile ? File { get; init; }

    public bool IsFile { get; init; }

    public IFormFile ? Image { get; init; }

    public bool IsImage { get; init; }

    public long StoreId { get; init; }

    public bool IsActive { get; init; }
}

public class UpdateStoreBioCommandHandler : IRequestHandler<UpdateStoreBioCommand, ApiResponse<StoreBioDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public UpdateStoreBioCommandHandler(ICatalogDbContext context, IStorageService storageService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public async Task<ApiResponse<StoreBioDto>> Handle(UpdateStoreBioCommand request, CancellationToken cancellationToken)
    {

        var entity = await _context.StoreBio
             .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(StoreBio), request.Id);
        }
        if (await _context.StoreBio.AsNoTracking().AnyAsync(x => x.StoreId == request.StoreId && x.Id != request.Id, cancellationToken))
        {
            return ApiResponse<StoreBioDto>.Error("Store already has a bio. Use update instead.");
        }
        entity.Text = request.Text;
        entity.StoreId = request.StoreId;
        entity.IsActive = request.IsActive;

        if (request.Image != null && request.Image.Length > 0)
        {
            if (!string.IsNullOrEmpty(entity.Image))
            {
                await _storageService.DeleteFileAsync(entity.Image, cancellationToken);
            }
            var newImagePath = await _storageService.SaveFileAsync(request.Image, cancellationToken);
            entity.Image = newImagePath;
        }
        else if (request.IsImage)
        {
            if (!string.IsNullOrEmpty(entity.Image))
                await _storageService.DeleteFileAsync(entity.Image, cancellationToken);
            entity.Image = string.Empty;
        }
        if (request.File != null && request.File.Length > 0)
        {
            if (!string.IsNullOrEmpty(entity.File))
            {
                await _storageService.DeleteFileAsync(entity.File, cancellationToken);
            }

            var newFilePath = await _storageService.SaveFileAsync(request.File, cancellationToken);
            entity.File = newFilePath;
        }
        else if (request.IsFile)
        {
            if (!string.IsNullOrEmpty(entity.File))
                await _storageService.DeleteFileAsync(entity.File, cancellationToken);
            entity.File = string.Empty;
        }
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<StoreBioDto>.Success(_mapper.Map<StoreBioDto>(entity));
    }
}