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

namespace WebApi.Controllers;


public class OrderController : ApiControllerBase
{
    /// <summary>
    /// Get orders.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<OrderDto>> GetOrders([FromQuery] GetOrdersQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Get order by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        return await Mediator.Send(new GetOrderQuery(id));
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Replace existing information about an order.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, UpdateOrderCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await Mediator.Send(command);

        //return NoContent();
    }

    /// <summary>
    /// Replace existing information about an order.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Order>> Delete(int id)
    {
        await Mediator.Send(new DeleteOrderCommand(id));
        return Ok();
        
    }

    /// <summary>
    /// Get list of item identifiers in the order cart.
    /// </summary>
    [HttpGet("{id}/Cart")]
    public async Task<IEnumerable<CartDto>> GetOrderCarts(int id)
    {
        return await Mediator.Send(new GetOrderCartsQuery(id));
    }

    /// <summary>
    /// Create a new item cart for the order.
    /// </summary>
    [HttpPost("{id}/Cart")]
    public async Task<ActionResult<CartDto>> AddOrderCart(int id, [FromBody] CartItemIdDto cartItemIdDto)
    {
        var cart = await Mediator.Send(new CreateOrderCartCommand(id, cartItemIdDto));
        await Mediator.Send(new UpdateOrderTotalCommand(id));
        return cart;
    }

    /// <summary>
    /// Get information about an item in the order cart.
    /// </summary>
    [HttpGet("{orderId}/Cart/{itemId}")]
    public async Task<ActionResult<CartDto>> GetOrderCart(int orderId, int itemId)
    {
        return await Mediator.Send(new GetOrderCartQuery(orderId, itemId));
    }

    /// <summary>
    /// Replace existing cart information.
    /// </summary>
    [HttpPut("{orderId}/Cart/{itemId}")]
    public async Task<ActionResult<CartDto>> UpdateOrderCart(int orderId, int itemId, [FromBody] CartBodyDto cartBodyDto)
    {
        var cart = await Mediator.Send(new UpdateOrderCartCommand(orderId, itemId, cartBodyDto));
        await Mediator.Send(new UpdateOrderTotalCommand(orderId));
        return cart;
    }

    /// <summary>
    /// Delete existing cart information.
    /// </summary>
    [HttpDelete("{orderId}/Cart/{itemId}")]
    public async Task<ActionResult<CartDto>> DeleteOrderCart(int orderId, int itemId)
    {
        var cart = await Mediator.Send(new DeleteOrderCartCommand(orderId, itemId));
        await Mediator.Send(new UpdateOrderTotalCommand(orderId));
        return Ok();
    }

    /// <summary>
    /// Get all delivery identifiers for the specified order.
    /// </summary>
    [HttpGet("{id}/Delivery")]
    public async Task<IEnumerable<DeliveryDto>> GetOrderDeliveries(int id)
    {
        return await Mediator.Send(new GetOrderDeliveryQuery(id));
    }

    /// <summary>
    /// Create a new delivery of the order.
    /// </summary>
    [HttpPost("{id}/Delivery")]
    public async Task<ActionResult<DeliveryDto>> AddOrderDelivery(int id, [FromBody] DeliveryBodyDto deliveryBodyDto)
    {
        return await Mediator.Send(new CreateOrderDeliveryCommand(id, deliveryBodyDto));
    }

    /// <summary>
    /// Get all payment identifiers for the specified order.
    /// </summary>
    [HttpGet("{id}/Payment")]
    public async Task<IEnumerable<PaymentDto>> GetOrderPayments(int id)
    {
        return await Mediator.Send(new GetOrderPaymentQuery(id));
    }

    /// <summary>
    /// Create a new payment of the order.
    /// </summary>
    [HttpPost("{id}/Payment")]
    public async Task<ActionResult<PaymentDto>> AddOrderPayment(int id, [FromBody] PaymentBodyDto deliveryBodyDto)
    {
        return await Mediator.Send(new CreateOrderPaymentCommand(id, deliveryBodyDto));
    }
}