using Application.Tenants.Commands.CreateTenantCommand;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;

namespace WebApi.Controllers;

public class TenantsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Tenant>> Create(CreateTenantCommand command)
    {
        var result = await Mediator.Send(command);
        return Created(result);
    }
}