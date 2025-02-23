using BuildingBlocks.Common.API.Controllers;
using MediatR;

namespace Catalog.Api.Controllers;

public abstract class ApiControllerBase: ApiController
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
