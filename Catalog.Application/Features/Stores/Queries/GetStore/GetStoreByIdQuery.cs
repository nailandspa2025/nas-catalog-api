using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetStore;

public class GetStoreByIdQuery: IRequest<ApiResponse<StoreDto>>
{
    public long Id { get; set; }
}

public class GetStoreByIdQueryHandler : IRequestHandler<GetStoreByIdQuery, ApiResponse<StoreDto>>
{
    private readonly ICatalogDbContext _contexxt;
    private readonly IMapper _mapper;

    public GetStoreByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _contexxt = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<StoreDto>> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _contexxt.Store
            .AsNoTracking()
            .Include(x => x.ImageGallerys)
            .Include(x => x.UserStores)
            .Include(x => x.BankAccounts)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if(entity == null)
        {
            throw new NotFoundException(nameof(Store), request.Id);
        }

        return ApiResponse<StoreDto>.Success(_mapper.Map<StoreDto>(entity));

    }
}
