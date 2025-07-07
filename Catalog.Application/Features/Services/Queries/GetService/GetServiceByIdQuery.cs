using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Services.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Services.Queries.GetService;

public record GetServiceByIdQuery: IRequest<ApiResponse<ServiceDto>>
{
    public int Id { get; init; }
}

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ApiResponse<ServiceDto>>
{
    private readonly ICatalogDbContext _contexxt;
    private readonly IMapper _mapper;

    public GetServiceByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _contexxt = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<ServiceDto>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _contexxt.Service
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }

        return ApiResponse<ServiceDto>.Success(_mapper.Map<ServiceDto>(entity));
    }
}