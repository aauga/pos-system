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

namespace WebApi.Controllers;


public class OrderController : ApiControllerBase
{
    /// <summary>
    /// Get orders.
    /// </summary>
    /// <response code="200">List of order identifiers was returned.</response>
    [HttpGet]
    public async Task<IEnumerable<OrderDto>> GetOrders([FromQuery] GetOrdersQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Get order by ID.
    /// </summary>
    /// <response code="200">Order was found and returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        return await Mediator.Send(new GetOrderQuery(id));
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <response code="201">Order was created and returned.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="403">Forbidden.</response>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderBodyDto orderBodyDto)
    {
        var order = await Mediator.Send(new CreateOrderCommand(orderBodyDto));
        return Created(order);
    }

    /// <summary>
    /// Replace existing information about an order.
    /// </summary>
    /// <response code="200">Order information was replaced and new information sent back.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Order was not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] OrderBodyDto orderBodyDto)
    {
        return await Mediator.Send(new UpdateOrderCommand(id, orderBodyDto));

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
    public async Task<IEnumerable<CartDto>> GetOrderCarts(int id)
    {
        return await Mediator.Send(new GetOrderCartsQuery(id));
    }

    /// <summary>
    /// Create a new item cart for the order.
    /// </summary>
    /// <response code="201">Order payment was created and returned.</response>
    /// <response code="403">Order cart for specified item already exists.</response>
    /// <response code="404">Order was not found.</response>
    [HttpPost("{id}/Cart")]
    public async Task<ActionResult<CartDto>> AddOrderCart(int id, [FromBody] CartItemIdDto cartItemIdDto)
    {
        var cart = await Mediator.Send(new CreateOrderCartCommand(id, cartItemIdDto));
        await Mediator.Send(new UpdateOrderTotalCommand(id));
        return Created(cart);
    }

    /// <summary>
    /// Get information about an item in the order cart.
    /// </summary>
    /// <response code="200">Cart information was found and returned.</response>
    /// <response code="404">Cart with order and item identifier combination was not found.</response>
    [HttpGet("{orderId}/Cart/{itemId}")]
    public async Task<ActionResult<CartDto>> GetOrderCart(int orderId, int itemId)
    {
        return await Mediator.Send(new GetOrderCartQuery(orderId, itemId));
    }

    /// <summary>
    /// Replace existing cart information.
    /// </summary>
    /// <response code="200">Cart information was replaced and returned.</response>
    /// <response code="404">Cart with order and item identifier combination was not found.</response>
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
    /// <response code="200">Cart information was deleted.</response>
    /// <response code="404">Cart with order and item identifier combination was not found.</response>
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
    /// <response code="200">List of identifiers was returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpGet("{id}/Delivery")]
    public async Task<IEnumerable<DeliveryDto>> GetOrderDeliveries(int id)
    {
        return await Mediator.Send(new GetOrderDeliveryQuery(id));
    }

    /// <summary>
    /// Create a new delivery of the order.
    /// </summary>
    /// <response code="201">Order delivery was created and returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpPost("{id}/Delivery")]
    public async Task<ActionResult<DeliveryDto>> AddOrderDelivery(int id, [FromBody] DeliveryBodyDto deliveryBodyDto)
    {
        var delivery = await Mediator.Send(new CreateOrderDeliveryCommand(id, deliveryBodyDto));
        return Created(delivery);
    }

    /// <summary>
    /// Get all payment identifiers for the specified order.
    /// </summary>
    /// <response code="200">List of identifiers was returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpGet("{id}/Payment")]
    public async Task<IEnumerable<PaymentDto>> GetOrderPayments(int id)
    {
        return await Mediator.Send(new GetOrderPaymentQuery(id));
    }

    /// <summary>
    /// Create a new payment of the order.
    /// </summary>
    /// <response code="201">Order payment was created and returned.</response>
    /// <response code="404">Order was not found</response>
    [HttpPost("{id}/Payment")]
    public async Task<ActionResult<PaymentDto>> AddOrderPayment(int id, [FromBody] PaymentBodyDto deliveryBodyDto)
    {
        var payment = await Mediator.Send(new CreateOrderPaymentCommand(id, deliveryBodyDto));
        return Created(payment);
    }
}