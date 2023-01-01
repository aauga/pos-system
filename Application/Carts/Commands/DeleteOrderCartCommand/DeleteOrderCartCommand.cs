using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Application.Orders.Commands.UpdateOrderCartTotalCommand;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts.Commands.DeleteOrderCartCommand;

public record DeleteOrderCartCommand(int OrderId, int ItemId) : IAuthorizedRequest
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}


public class DeleteOrderCartCommandHandler : IRequestHandler<DeleteOrderCartCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteOrderCartCommand request, CancellationToken cancellationToken)
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

        _dbContext.Carts.Remove(entity);

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

        return Unit.Value;
    }
}
