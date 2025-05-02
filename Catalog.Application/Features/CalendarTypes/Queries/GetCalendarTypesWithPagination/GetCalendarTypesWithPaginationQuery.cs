using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.CalendarTypes.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.CalendarTypes.Queries.GetCalendarTypesWithPagination;

public record GetCalendarTypesWithPaginationQuery: IRequest<ApiResponse<PaginatedList<CalendarTypeDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

public class GetCalendarTypesWithPaginationQueryHandler : IRequestHandler<GetCalendarTypesWithPaginationQuery, ApiResponse<PaginatedList<CalendarTypeDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetCalendarTypesWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }
    public async Task<ApiResponse<PaginatedList<CalendarTypeDto>>> Handle(GetCalendarTypesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.CalendarType.Where(x => !x.IsDeleted && x.CreatedBy == _currentUser.UserName ).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.Contains(s.Name));
        }

        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<CalendarTypeDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<CalendarTypeDto>>.Success(paginationResult);
    }
}
