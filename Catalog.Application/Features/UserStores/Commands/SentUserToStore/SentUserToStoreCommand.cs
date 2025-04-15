using Amazon.CloudWatchLogs.Model;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.UserStores.Commands.SentUserToStore;

public record SentUserToStoreCommand: IRequest<Unit>
{
	public string UserId { get; init; }

	public List<long> StoreIds { get; init; }
}

public class SentUserToStoreCommandHandler : IRequestHandler<SentUserToStoreCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public SentUserToStoreCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }
    public async Task<Unit> Handle(SentUserToStoreCommand request, CancellationToken cancellationToken)
    {
        var userStores = await _context.UserStore
        .Where(us => us.UserId == request.UserId)
        .ToListAsync(cancellationToken);

        _context.UserStore.RemoveRange(userStores);

        if (request.StoreIds != null && request.StoreIds.Any())
        {
            var newUserStores = request.StoreIds.Select(storeId => new UserStore
            {
                UserId = request.UserId,
                StoreId = storeId
            }).ToList();

            _context.UserStore.AddRange(newUserStores);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

