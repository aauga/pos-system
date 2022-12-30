using Application.Items;
using Application.Orders;
using CleanArchitecture.WebUI.Controllers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class ItemController : ApiControllerBase
{
    /// <summary>
    /// Get all items.
    /// </summary>
    /// <response code="200">List of items identifiers was returned.</response>
    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetItems([FromQuery] GetItemsQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Get item by ID.
    /// </summary>
    /// <response code="200">Information about item was returned.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Item with specified identifier was not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        return await Mediator.Send(new GetItemQuery(id));
    }

    /// <summary>
    /// Create a new item.
    /// </summary>
    /// <response code="200">Item was created and returned.</response>
    /// <response code="403">Forbidden.</response>
    [HttpPost]
    public async Task<ActionResult<ItemDto>> Create(CreateItemCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Replace existing information about an item.
    /// </summary>
    /// <response code="200">Item information was replaced and new information sent back.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Item was not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ItemDto>> Update(int id, [FromBody] ItemBodyDto itemDto)
    {
        return await Mediator.Send(new UpdateItemCommand(id, itemDto));
    }
}