using Application.Items;
using Application.Orders;
using CleanArchitecture.WebUI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class ItemController : ApiControllerBase
{
    /// <summary>
    /// Create a new item.
    /// </summary>
    /// <response code="200">Item was created and returned.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="403">Forbidden.</response>
    [HttpPost]
    public async Task<ActionResult<ItemDto>> Create(CreateItemCommand command)
    {
        return await Mediator.Send(command);
    }
}