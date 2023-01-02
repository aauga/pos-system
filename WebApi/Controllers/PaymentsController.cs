using Application.Payments;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WebUI.Controllers;
using Application.Common.Attributes;

namespace WebApi.Controllers;


public class PaymentController : ApiControllerBase
{
    [HttpGet]
    [Summary("Get payments.")]
    public async Task<IEnumerable<PaymentDto>> GetPaymments([FromQuery] GetPaymentsQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Summary("Get payment by ID.")]
    public async Task<ActionResult<PaymentDto>> GetPayment(int id)
    {
        return await Mediator.Send(new GetPaymentQuery(id));
    }

    [HttpPut("{id}")]
    [Summary("Replace existing information about a payment.")]
    public async Task<ActionResult<PaymentDto>> Update(int id, UpdatePaymentCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    [Summary("Delete payment.")]
    public async Task<ActionResult<Order>> Delete(int id)
    {
        await Mediator.Send(new DeletePaymentCommand(id));
        return Ok();

    }
}