using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Orders.Commands.UpdateOrderCartTotalCommand;

using System.Net.Sockets;

namespace Application.Carts.Commands.CreateOrderCartCommand;

public record CreateOrderCartCommand(int orderId, CartItemIdDto cartItemIdDto) : IAuthorizedRequest<CartDto>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.orderId);

        if (order == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        /*var cart = await _dbContext.Carts
            .Where(b => b.OrderId == request.orderId && b.ItemId == request.cartItemIdDto.ItemId)
            .ToListAsync();

        if (cart.Count != 0)
        {
            throw new ForbiddenAccessException();
        }*/

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

        var cartDto = new CartDto(entity);

        var updater = new UpdateOrderTotal(entity.OrderId, _dbContext);
        await updater.Update();

        return cartDto;
    }
}
