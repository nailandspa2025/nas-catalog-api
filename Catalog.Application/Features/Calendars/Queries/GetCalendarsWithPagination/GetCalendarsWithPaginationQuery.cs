using BuildingBlocks.Core.Response;
using Catalog.Application.Features.CalendarTypes.Models;
using MediatR;

namespace Catalog.Application.Features.Calendars.Queries.GetCalendarsWithPagination;

public record GetCalendarsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<CalendarTypeDto>>>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SearchText { get; set; }
}

