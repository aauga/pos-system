using Application.Employees.Commands.CreateEmployeeCommand;
using Application.Employees.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;

namespace WebApi.Controllers;

public class EmployeesController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeCommand command)
    {
        var result = await Mediator.Send(command);
        return Created(result);
    }
}