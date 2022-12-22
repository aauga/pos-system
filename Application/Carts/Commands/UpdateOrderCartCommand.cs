using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts;

public record UpdateOrderCartCommand(int orderId, int itemId, CartBodyDTO cartBodyDTO) : IRequest<CartDTO>;


public class UpdateOrderCartCommandHandler : IRequestHandler<UpdateOrderCartCommand, CartDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDTO> Handle(UpdateOrderCartCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Carts
             .SingleAsync(b => b.OrderId == request.orderId && b.ItemId == request.itemId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        entity.Quantity = request.cartBodyDTO.Quantity;
        entity.Discount = request.cartBodyDTO.Discount;
        entity.Description = request.cartBodyDTO.Description;

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
