using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts;

public record CreateOrderCartCommand(int orderId, CartItemIdDTO cartItemIdDTO) : IRequest<CartDTO>;


public class CreateOrderCartCommandHandler : IRequestHandler<CreateOrderCartCommand, CartDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDTO> Handle(CreateOrderCartCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(new object[] { request.orderId }, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var entity = new Cart
        {
            Id = 0,
            OrderId = request.orderId,
            ItemId = request.cartItemIdDTO.ItemId,
            Quantity = request.cartItemIdDTO.Quantity,
            Discount = request.cartItemIdDTO.Discount,
            Description = request.cartItemIdDTO.Description
        };
        

        _dbContext.Carts.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var carts = _dbContext.Carts
            .Where(b => b.OrderId == request.orderId)
            .ToList();

        decimal total = 0;

        foreach (var cart in carts)
        {
            var item = _dbContext.Items
                .Single(b => b.Id == cart.ItemId);
            if (cart.Discount == 0)
            {
                total += item.Price * (decimal)cart.Quantity;
            }
            else
            {
                total += item.Price * (decimal)cart.Quantity / cart.Discount;
            }
        }

        order.Total = total;

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var deliveryDTO = new CartDTO
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            ItemId = entity.ItemId,
            Quantity = entity.Quantity,
            Discount = entity.Discount,
            Description = entity.Description
        };

        return deliveryDTO;
    }
}
