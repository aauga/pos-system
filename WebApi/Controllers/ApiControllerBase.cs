using MediatR;

using Microsoft.AspNetCore.Mvc;
using WebAPI.Filters;

namespace CleanArchitecture.WebUI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ApiExceptionFilterAttribute]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}