using Application.Orders;
using Application.Deliveries;
using Application.Payments;
using Application.Carts;
using CleanArchitecture.WebUI.Controllers;
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
    /// <response code="200">List of order identifiers was returned.</response>
    [HttpGet]
    public async Task<IEnumerable<OrderDTO>> GetOrders([FromQuery] GetOrdersQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Get order by ID.
    /// </summary>
    /// <response code="200">Order was found and returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        return await Mediator.Send(new GetOrderQuery(id));
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <response code="200">Order was created and returned.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="403">Forbidden.</response>
    [HttpPost]
    public async Task<ActionResult<OrderDTO>> Create(CreateOrderCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Replace existing information about an order.
    /// </summary>
    /// <response code="200">Order information was replaced and new information sent back.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Order was not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDTO>> Update(int id, UpdateOrderCommand command)
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
    /// <response code="200">Information was deleted successfully.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Order was not found.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Order>> Delete(int id)
    {
        await Mediator.Send(new DeleteOrderCommand(id));
        return Ok();
        
    }

    /// <summary>
    /// Get list of item identifiers in the order cart.
    /// </summary>
    /// <response code="200">List of identifiers was returned.</response>
    /// <response code="404">Order was not found.</response>
    [HttpGet("{id}/Cart")]
    public async Task<IEnumerable<CartDTO>> GetOrderCarts(int id)
    {
        return await Mediator.Send(new GetOrderCartsQuery(id));
    }

    /// <summary>
    /// Create a new item cart for the order.
    /// </summary>
    /// <response code="200">Order payment was created and returned.</response>
    /// <response code="403">Order cart for specified item already exists.</response>
    /// <response code="404">Order was not found.</response>
    [HttpPost("{id}/Cart")]
    public async Task<ActionResult<CartDTO>> AddOrderCart(int id, [FromBody] CartItemIdDTO cartItemIdDTO)
    {
        var cart = await Mediator.Send(new CreateOrderCartCommand(id, cartItemIdDTO));
        await Mediator.Send(new UpdateOrderTotalCommand(id));
        return cart;
    }

    /// <summary>
    /// Get information about an item in the order cart.
    /// </summary>
    /// <response code="200">Cart information was found and returned.</response>
    /// <response code="404">Cart with order and item identifier combination was not found.</response>
    [HttpGet("{orderId}/Cart/{itemId}")]
    public async Task<ActionResult<CartDTO>> GetOrderCart(int orderId, int itemId)
    {
        return await Mediator.Send(new GetOrderCartQuery(orderId, itemId));
    }

    /// <summary>
    /// Replace existing cart information.
    /// </summary>
    /// <response code="200">Cart information was replaced and returned.</response>
    /// <response code="404">Cart with order and item identifier combination was not found.</response>
    [HttpPut("{orderId}/Cart/{itemId}")]
    public async Task<ActionResult<CartDTO>> UpdateOrderCart(int orderId, int itemId, [FromBody] CartBodyDTO cartBodyDTO)
    {
        var cart = await Mediator.Send(new UpdateOrderCartCommand(orderId, itemId, cartBodyDTO));
        await Mediator.Send(new UpdateOrderTotalCommand(orderId));
        return cart;
    }

    /// <summary>
    /// Delete existing cart information.
    /// </summary>
    /// <response code="200">Cart information was deleted.</response>
    /// <response code="404">Cart with order and item identifier combination was not found.</response>
    [HttpDelete("{orderId}/Cart/{itemId}")]
    public async Task<ActionResult<CartDTO>> DeleteOrderCart(int orderId, int itemId)
    {
        var cart = await Mediator.Send(new DeleteOrderCartCommand(orderId, itemId));
        await Mediator.Send(new UpdateOrderTotalCommand(orderId));
        return Ok();
    }

    /// <summary>
    /// Get all delivery identifiers for the specified order.
    /// </summary>
    /// <response code="200">List of identifiers was returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpGet("{id}/Delivery")]
    public async Task<IEnumerable<DeliveryDTO>> GetOrderDeliveries(int id)
    {
        return await Mediator.Send(new GetOrderDeliveryQuery(id));
    }

    /// <summary>
    /// Create a new delivery of the order.
    /// </summary>
    /// <response code="200">Order delivery was created and returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpPost("{id}/Delivery")]
    public async Task<ActionResult<DeliveryDTO>> AddOrderDelivery(int id, [FromBody] DeliveryBodyDTO deliveryBodyDTO)
    {
        return await Mediator.Send(new CreateOrderDeliveryCommand(id, deliveryBodyDTO));
    }

    /// <summary>
    /// Get all payment identifiers for the specified order.
    /// </summary>
    /// <response code="200">List of identifiers was returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpGet("{id}/Payment")]
    public async Task<IEnumerable<PaymentDTO>> GetOrderPayments(int id)
    {
        return await Mediator.Send(new GetOrderPaymentQuery(id));
    }

    /// <summary>
    /// Create a new payment of the order.
    /// </summary>
    /// <response code="200">Order payment was created and returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpPost("{id}/Payment")]
    public async Task<ActionResult<PaymentDTO>> AddOrderPayment(int id, [FromBody] PaymentBodyDTO deliveryBodyDTO)
    {
        return await Mediator.Send(new CreateOrderPaymentCommand(id, deliveryBodyDTO));
    }
}