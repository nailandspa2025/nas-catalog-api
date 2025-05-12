using BuildingBlocks.Core.Response;
using Catalog.Application.Features.CalendarTypes.Models;
using MediatR;

namespace Catalog.Application.Features.Calendars.Queries.GetCalendarsWithPagination;

public record GetCalendarsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<CalendarTypeDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

