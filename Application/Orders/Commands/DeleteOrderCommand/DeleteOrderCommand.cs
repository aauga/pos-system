using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.DeleteOrderCommand;

public record DeleteOrderCommand(int Id) : IAuthorizedRequest
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(Id);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Orders.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }
        _dbContext.Orders.Remove(entity);

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
