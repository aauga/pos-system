using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Application.Orders.Commands.UpdateOrderCartTotalCommand;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts.Commands.UpdateOrderCartCommand;

public record UpdateOrderCartCommand(int orderId, int itemId, CartBodyDto cartBodyDto) : IAuthorizedRequest<CartDto>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.orderId);

        if (order == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

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

        var updater = new UpdateOrderTotal(entity.OrderId, _dbContext);
        await updater.Update();

        var cartDto = new CartDto(entity);

        return cartDto;
    }
}
