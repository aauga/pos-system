using Application.Employees;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;

namespace WebApi.Controllers;

public class EmployeeController : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee([FromQuery] GetEmployeeQuery query)
    {
        return await Mediator.Send(query);
    }
}