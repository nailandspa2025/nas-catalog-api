using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;
namespace Catalog.Application.Features.Rewards.Commands.DeleteReward;

public record DeleteRewardCommand(int Id): IRequest<ApiResponse>;

public class DeleteRewardCommandHandler : IRequestHandler<DeleteRewardCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteRewardCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteRewardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Reward
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Reward), request.Id);
        }

        _context.Reward.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}

