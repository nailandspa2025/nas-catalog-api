using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.CalendarTypes.Commands.DeleteCalendarType;

public record DeleteCalendarTypeCommand(int Id): IRequest<ApiResponse>;

public class DeleteCalendarTypeCommandHandler : IRequestHandler<DeleteCalendarTypeCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteCalendarTypeCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteCalendarTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CalendarType
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(CalendarType), request.Id);
        }

        _context.CalendarType.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse.Success();
    }
}