using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.UserStores.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.UserStores.Queries.GetUserStore;

public record GetUserStoreByUserIdQuery: IRequest<ApiResponse<IEnumerable<UserStoreDto>>>
{
	public string UserId { get; init; } = null!;
}

public class GetUserStoreByUserIdQueryHandler : IRequestHandler<GetUserStoreByUserIdQuery, ApiResponse<IEnumerable<UserStoreDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetUserStoreByUserIdQueryHandler( ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<UserStoreDto>>> Handle(GetUserStoreByUserIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await _context.UserStore
            .Where(us => us.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        //if (!entities.Any())
        //{
        //    throw new NotFoundException(nameof(UserStore), request.UserId);
        //}
        return ApiResponse<IEnumerable<UserStoreDto>>.Success(_mapper.Map<IEnumerable<UserStoreDto>>(entities));
    }
}
