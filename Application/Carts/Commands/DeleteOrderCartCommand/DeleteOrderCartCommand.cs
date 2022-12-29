using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Carts.Commands.DeleteOrderCartCommand;

public record DeleteOrderCartCommand(int orderId, int itemId) : IRequest;


public class DeleteOrderCartCommandHandler : IRequestHandler<DeleteOrderCartCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteOrderCartCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteOrderCartCommand request, CancellationToken cancellationToken)
    {
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
