using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts.Commands.UpdateOrderCartCommand;

public record UpdateOrderCartCommand(int orderId, int itemId, CartBodyDto cartBodyDto) : IRequest<CartDto>;


public class UpdateOrderCartCommandHandler : IRequestHandler<UpdateOrderCartCommand, CartDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDto> Handle(UpdateOrderCartCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Carts
             .SingleAsync(b => b.OrderId == request.orderId && b.ItemId == request.itemId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        entity.Quantity = request.cartBodyDto.Quantity;
        entity.Discount = request.cartBodyDto.Discount;
        entity.Description = request.cartBodyDto.Description;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var cartDto = new CartDto(entity);

        return cartDto;
    }
}
