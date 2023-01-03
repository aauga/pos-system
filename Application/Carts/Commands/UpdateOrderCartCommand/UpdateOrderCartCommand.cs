using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Application.Orders.Commands.UpdateOrderCartTotalCommand;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts.Commands.UpdateOrderCartCommand;

public record UpdateOrderCartCommand(int OrderId, int ItemId, CartBodyDto cartBodyDto) : IAuthorizedRequest<CartDto>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}

public class UpdateOrderCartCommandHandler : IRequestHandler<UpdateOrderCartCommand, CartDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDto> Handle(UpdateOrderCartCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var entity = await _dbContext.Carts
             .SingleAsync(b => b.OrderId == request.OrderId && b.ItemId == request.ItemId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
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

        var updater = new UpdateOrderTotal(entity.OrderId, _dbContext);
        await updater.Update();

        var cartDto = new CartDto(entity);

        return cartDto;
    }
}
