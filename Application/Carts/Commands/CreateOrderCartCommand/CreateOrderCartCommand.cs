using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Orders.Commands.UpdateOrderCartTotalCommand;

using System.Net.Sockets;

namespace Application.Carts.Commands.CreateOrderCartCommand;

public record CreateOrderCartCommand(int OrderId, CartItemIdDto cartItemIdDto) : IAuthorizedRequest<CartDto>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}


public class CreateOrderCartCommandHandler : IRequestHandler<CreateOrderCartCommand, CartDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDto> Handle(CreateOrderCartCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var entity = new Cart
        {
            OrderId = request.OrderId,
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

        var cartDto = new CartDto(entity);

        var updater = new UpdateOrderTotal(entity.OrderId, _dbContext);
        await updater.Update();

        return cartDto;
    }
}
