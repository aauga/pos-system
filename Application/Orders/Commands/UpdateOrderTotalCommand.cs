using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public record UpdateOrderTotalCommand (int id) : IRequest;

public class UpdateOrderTotalCommandHandler : IRequestHandler<UpdateOrderTotalCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderTotalCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(UpdateOrderTotalCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.id);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.id);
        }

        var carts = _dbContext.Carts
            .Where(b => b.OrderId == request.id)
            .ToList();

        decimal total = 0;

        foreach (var cart in carts)
        {
            var item = _dbContext.Items
                .Single(b => b.Id == cart.ItemId);
            if (cart.Discount == 0)
            {
                total += item.Price * (decimal)cart.Quantity;
            }
            else
            {
                total += item.Price * (decimal)cart.Quantity * (100 - cart.Discount)/100;
            }
        }

        order.Total = total;

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
