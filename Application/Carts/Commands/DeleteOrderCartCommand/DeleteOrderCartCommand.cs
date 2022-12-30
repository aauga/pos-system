using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts.Commands.DeleteOrderCartCommand;

public record DeleteOrderCartCommand(int orderId, int itemId) : IAuthorizedRequest
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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

        _dbContext.Carts.Remove(entity);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        return Unit.Value;
    }
}
