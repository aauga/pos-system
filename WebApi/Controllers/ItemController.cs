using Application.Items;
using Application.Orders;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;

namespace WebApi.Controllers;

[Authorize]
public class ItemController : ApiControllerBase
{
    /// <summary>
    /// Get all items.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetItems([FromQuery] GetItemsQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Get item by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        return await Mediator.Send(new GetItemQuery(id));
    }

    /// <summary>
    /// Create a new item.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ItemDto>> Create(CreateItemCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Replace existing information about an item.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ItemDto>> Update(int id, [FromBody] ItemBodyDto itemDto)
    {
        return await Mediator.Send(new UpdateItemCommand(id, itemDto));
    }

    /// <summary>
    /// Delete item.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteItemCommand(id));
        return Ok();
    }
}