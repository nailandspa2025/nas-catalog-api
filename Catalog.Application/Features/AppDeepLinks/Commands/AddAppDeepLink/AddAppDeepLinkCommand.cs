using Amazon.CloudWatchLogs.Model;
using AutoMapper;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.AppDeepLinks.Commands.AddAppDeepLink;

public record AddAppDeepLinkCommand(long StoreId) : IRequest<ApiResponse>;
public class AddAppDeepLinkCommandHandler : IRequestHandler<AddAppDeepLinkCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;
    private readonly ICurrentUser _currentUser;
    public AddAppDeepLinkCommandHandler(ICatalogDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<ApiResponse> Handle(AddAppDeepLinkCommand request, CancellationToken cancellationToken)
    {

        bool alreadyExists = await _context.UserStoreDeepLink
               .AnyAsync(x => x.UserId == _currentUser.UserId && x.StoreId == request.StoreId, cancellationToken);
        if (alreadyExists)
        {
            return ApiResponse.Success("Already exists");
        }
        var entity = new UserStoreDeepLink
        {
            UserId = _currentUser.UserId,
            StoreId = request.StoreId,
        };
        _context.UserStoreDeepLink.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse.Success();
    }
}
