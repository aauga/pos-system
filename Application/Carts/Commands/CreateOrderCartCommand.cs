using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts;

public record CreateOrderCartCommand(int orderId, CartItemIdDto cartItemIdDto) : IRequest<CartDto>;


public class CreateOrderCartCommandHandler : IRequestHandler<CreateOrderCartCommand, CartDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDto> Handle(CreateOrderCartCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var cart = await _dbContext.Carts
            .Where(b => b.OrderId == request.orderId && b.ItemId == request.cartItemIdDto.ItemId)
            .ToListAsync();
             //.SingleAsync(b => b.OrderId == request.orderId && b.ItemId == request.cartItemIdDto.ItemId);

        if (cart.Count != 0)
        {
            throw new ForbiddenAccessException();
        }

        var entity = new Cart
        {
            OrderId = request.orderId,
            ItemId = request.cartItemIdDto.ItemId,
            Quantity = request.cartItemIdDto.Quantity,
            Discount = request.cartItemIdDto.Discount,
            Description = request.cartItemIdDto.Description
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
        
        var cartDto = new CartDto
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            ItemId = entity.ItemId,
            Quantity = entity.Quantity,
            Discount = entity.Discount,
            Description = entity.Description
        };

        return cartDto;
    }
}
