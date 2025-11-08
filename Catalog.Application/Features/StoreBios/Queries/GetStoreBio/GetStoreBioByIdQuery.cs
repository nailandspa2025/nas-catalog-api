using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.StoreBios.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.StoreBios.Queries.GetStoreBio;

public record GetStoreBioByIdQuery: IRequest<ApiResponse<StoreBioDto>>
{
	public int Id { get; init; }
}

public class GetStoreBioByIdQueryHandler : IRequestHandler<GetStoreBioByIdQuery, ApiResponse<StoreBioDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetStoreBioByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<StoreBioDto>> Handle(GetStoreBioByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.StoreBio
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(StoreBio), request.Id);
        }

        return ApiResponse<StoreBioDto>.Success(_mapper.Map<StoreBioDto>(entity));
    }
}