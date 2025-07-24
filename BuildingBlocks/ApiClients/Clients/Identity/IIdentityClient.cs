using BuildingBlocks.ApiClients.Clients.Identity.Models;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.ApiClients.Clients.Identity;

public interface IIdentityClient
{
    [Refit.Get("/api/v1/appaccounts/ids")]
    Task<ApiResponse<IEnumerable<AppAccountDto>>> GetAppAccountByIdsAsync(string ids ,CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/technicians/storeid/{storeId}")]
    Task<ApiResponse<IEnumerable<TechnicianDto>>> GetTechniciansByStoreIdAsync(long storeId, CancellationToken cancellationToken = default);
}

