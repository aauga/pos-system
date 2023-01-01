using Application.Common.Attributes;
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
    [HttpGet("Tenant/{tenantId}")]
    [Summary("Get all items of tenant.")]
    public async Task<IEnumerable<ItemDto>> GetItems(int tenantId, [FromQuery] GetItemsQuery query)
    {
        query.TenantId = tenantId;
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Summary("Get item by ID.")]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        return await Mediator.Send(new GetItemQuery(id));
    }

    [HttpPost("Tenant/{tenantId}")]
    [Summary("Create a new item.")]
    public async Task<ActionResult<ItemDto>> Create(int tenantId, [FromBody] CreateItemCommand command)
    {
        command.TenantId = tenantId;
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [Summary("Replace existing information about an item.")]
    public async Task<ActionResult<ItemDto>> Update(int id, [FromBody] ItemBodyDto itemDto)
    {
        return await Mediator.Send(new UpdateItemCommand(id, itemDto));
    }

    [HttpDelete("{id}")]
    [Summary("Delete item.")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteItemCommand(id));
        return Ok();
    }
}