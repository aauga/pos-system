using Application.Orders;
using Application.Deliveries;
using Application.Payments;
using Application.Carts;
using WebUI.Controllers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Net;
using WebAPI.Filters;
using Application.Orders.Commands.CreateOrderCommand;
using Application.Carts.Commands.CreateOrderCartCommand;
using Application.Carts.Commands.UpdateOrderCartCommand;
using Application.Carts.Commands.DeleteOrderCartCommand;
using Application.Orders.Commands.UpdateOrderCommand;
using Application.Orders.Commands.DeleteOrderCommand;
using Application.Orders.Commands.UpdateOrderCartTotalCommand;
using Application.Deliveries.Commands.CreateOrderDeliveryCommand;
using Application.Payments.Commands.CreateOrderPaymentCommand;
using Swashbuckle.AspNetCore.Annotations;
using Application.Common.Attributes;

namespace WebApi.Controllers;


public class OrderController : ApiControllerBase
{
    [HttpGet]
    [Summary("Get orders.")]
    public async Task<IEnumerable<OrderDto>> GetOrders([FromQuery] GetOrdersQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Summary("Get order by ID.")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        return await Mediator.Send(new GetOrderQuery(id));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderBodyDto orderBodyDto)
    [Summary("Create a new order.")]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderCommand command)
    {
        var order = await Mediator.Send(new CreateOrderCommand(orderBodyDto));
        return Created(order);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] OrderBodyDto orderBodyDto)
    [Summary("Replace existing information about an order.")]
    public async Task<ActionResult<OrderDto>> Update(int id, UpdateOrderCommand command)
    {
        return await Mediator.Send(new UpdateOrderCommand(id, orderBodyDto));

        //return NoContent();
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    [Summary("Replace existing information about an order.")]
    public async Task<ActionResult<Order>> Delete(int id)
    {
        await Mediator.Send(new DeleteOrderCommand(id));
        return Ok();
        
    }

    [HttpGet("{id}/Cart")]
    [Summary("Get list of item identifiers in the order cart.")]
    public async Task<IEnumerable<CartDto>> GetOrderCarts(int id)
    {
        return await Mediator.Send(new GetOrderCartsQuery(id));
    }

    [HttpPost("{id}/Cart")]
    [Summary("Create a new item cart for the order.")]
    public async Task<ActionResult<CartDto>> AddOrderCart(int id, [FromBody] CartItemIdDto cartItemIdDto)
    {
        var cart = await Mediator.Send(new CreateOrderCartCommand(id, cartItemIdDto));
        await Mediator.Send(new UpdateOrderTotalCommand(id));
        return Created(cart);
    }

    [HttpGet("{orderId}/Cart/{itemId}")]
    [Summary("Get information about an item in the order cart.")]
    public async Task<ActionResult<CartDto>> GetOrderCart(int orderId, int itemId)
    {
        return await Mediator.Send(new GetOrderCartQuery(orderId, itemId));
    }

    [HttpPut("{orderId}/Cart/{itemId}")]
    [Summary("Replace existing cart information.")]
    public async Task<ActionResult<CartDto>> UpdateOrderCart(int orderId, int itemId, [FromBody] CartBodyDto cartBodyDto)
    {
        var cart = await Mediator.Send(new UpdateOrderCartCommand(orderId, itemId, cartBodyDto));
        await Mediator.Send(new UpdateOrderTotalCommand(orderId));
        return cart;
    }

    [HttpDelete("{orderId}/Cart/{itemId}")]
    [Summary("Delete existing cart information.")]
    public async Task<ActionResult<CartDto>> DeleteOrderCart(int orderId, int itemId)
    {
        var cart = await Mediator.Send(new DeleteOrderCartCommand(orderId, itemId));
        await Mediator.Send(new UpdateOrderTotalCommand(orderId));
        return Ok();
    }

    [HttpGet("{id}/Delivery")]
    [Summary("Get all delivery identifiers for the specified order.")]
    public async Task<IEnumerable<DeliveryDto>> GetOrderDeliveries(int id)
    {
        return await Mediator.Send(new GetOrderDeliveryQuery(id));
    }

    [HttpPost("{id}/Delivery")]
    [Summary("Create a new delivery of the order.")]
    public async Task<ActionResult<DeliveryDto>> AddOrderDelivery(int id, [FromBody] DeliveryBodyDto deliveryBodyDto)
    {
        var delivery = await Mediator.Send(new CreateOrderDeliveryCommand(id, deliveryBodyDto));
        return Created(delivery);
    }

    [HttpGet("{id}/Payment")]
    [Summary("Get all payment identifiers for the specified order.")]
    public async Task<IEnumerable<PaymentDto>> GetOrderPayments(int id)
    {
        return await Mediator.Send(new GetOrderPaymentQuery(id));
    }

    [HttpPost("{id}/Payment")]
    [Summary("Create a new payment of the order")]
    public async Task<ActionResult<PaymentDto>> AddOrderPayment(int id, [FromBody] PaymentBodyDto deliveryBodyDto)
    {
        var payment = await Mediator.Send(new CreateOrderPaymentCommand(id, deliveryBodyDto));
        return Created(payment);
    }
}