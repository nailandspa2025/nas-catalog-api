using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Uploads.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Features.Uploads.Commands.CreateUpload;

public record CreateUploadCommand: IRequest<ApiResponse<UploadDto>>
{
	public IFormFile File { get; init; } = null!;
}

public class CreateUploadCommandHandler : IRequestHandler<CreateUploadCommand, ApiResponse<UploadDto>>
{
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public CreateUploadCommandHandler (IStorageService storageService, IMapper mapper)
    {
        _mapper = mapper;
        _storageService = storageService;
    }
    public async Task<ApiResponse<UploadDto>> Handle(CreateUploadCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return ApiResponse<UploadDto>.Error("File không hợp lệ hoặc rỗng");
        }
        var fileUrl = await _storageService.SaveFileAsync(request.File, cancellationToken);

        var uploadDto = new UploadDto { Url = fileUrl };

        return ApiResponse<UploadDto>.Success(uploadDto);
    }
}
