using Application.Customers.Commands.CreateCustomerCommand;
using Application.Customers.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;

namespace WebApi.Controllers;

[Authorize]
public class CustomersController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerCommand command)
    {
        var result = await Mediator.Send(command);
        return Created(result);
    }
}