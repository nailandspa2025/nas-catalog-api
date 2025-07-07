using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ServicePackages.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ServicePackages.Queries.GetServicePackage;

public record GetServicePackageByIdQuery : IRequest<ApiResponse<ServicePackageDto>>
{
    public int Id { get; init; }
}

public class GetServicePackageByIdQueryHandler : IRequestHandler<GetServicePackageByIdQuery, ApiResponse<ServicePackageDto>>
{
    private readonly ICatalogDbContext _contexxt;
    private readonly IMapper _mapper;

    public GetServicePackageByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _contexxt = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<ServicePackageDto>> Handle(GetServicePackageByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _contexxt.ServicePackage
            .Include(x =>x.Services)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(ServicePackage), request.Id);
        }

        return ApiResponse<ServicePackageDto>.Success(_mapper.Map<ServicePackageDto>(entity));
    }
}