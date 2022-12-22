using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
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
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var cart = await _dbContext.Carts
            .Where(b => b.OrderId == request.orderId && b.ItemId == request.cartItemIdDTO.ItemId)
            .ToListAsync();
             //.SingleAsync(b => b.OrderId == request.orderId && b.ItemId == request.cartItemIdDTO.ItemId);

        if (cart.Count != 0)
        {
            throw new ForbiddenAccessException();
        }

        var entity = new Cart
        {
            OrderId = request.orderId,
            ItemId = request.cartItemIdDTO.ItemId,
            Quantity = request.cartItemIdDTO.Quantity,
            Discount = request.cartItemIdDTO.Discount,
            Description = request.cartItemIdDTO.Description
        };
        
        _dbContext.Carts.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }
        
        var cartDTO = new CartDTO
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            ItemId = entity.ItemId,
            Quantity = entity.Quantity,
            Discount = entity.Discount,
            Description = entity.Description
        };

        return cartDTO;
    }
}
