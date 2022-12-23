using Domain.Common;
using MediatR;

using Microsoft.AspNetCore.Mvc;
using WebAPI.Filters;

namespace WebUI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ApiExceptionFilterAttribute]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    public CreatedResult Created(object? value)
    {
        var request = HttpContext.Request;
        var id = (value as BaseEntity)?.Id;

        var uri = string.Format("{0}://{1}{2}/{3}",
            request.Scheme,
            request.Host,
            request.Path,
            id
        );

        return Created(uri, value);
    }
}