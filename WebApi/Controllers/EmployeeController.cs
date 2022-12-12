using Application.Employees;
using CleanArchitecture.WebUI.Controllers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class EmployeeController : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee([FromQuery] GetEmployeeQuery query)
    {
        return await Mediator.Send(query);
    }
}