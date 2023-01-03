using Application.Deliveries;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;
using Application.Common.Attributes;

namespace WebApi.Controllers;


public class DeliveryController : ApiControllerBase
{
    [HttpGet]
    [Summary("Get deliveries.")]
    public async Task<IEnumerable<DeliveryDto>> GetPaymments([FromQuery] GetDeliveriesQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Summary("Get delivery by ID.")]
    public async Task<ActionResult<DeliveryDto>> GetDelivery(int id)
    {
        return await Mediator.Send(new GetDeliveryQuery(id));
    }

    [HttpPut("{id}")]
    [Summary("Replace existing information about a delivery.")]
    public async Task<ActionResult<DeliveryDto>> Update(int id, UpdateDeliveryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    [Summary("Delete delivery.")]
    public async Task<ActionResult<Delivery>> Delete(int id)
    {
        await Mediator.Send(new DeleteDeliveryCommand(id));
        return Ok();

    }
}