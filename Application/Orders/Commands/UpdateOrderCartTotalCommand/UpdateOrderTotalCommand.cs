using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.UpdateOrderCartTotalCommand;

public class UpdateOrderTotal
{
    private int Id;
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderTotal(int id, IApplicationDbContext dbContext)
    {
        Id = id;
        _dbContext = dbContext;
    }

    public async Task<Unit> Update()
    {
        var order = await _dbContext.Orders.FindAsync(Id);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), Id);
        }

        var carts = _dbContext.Carts
            .Where(b => b.OrderId == Id)
            .ToList();

        decimal total = 0;

        foreach (var cart in carts)
        {
            var item = _dbContext.Items
                .Single(b => b.Id == cart.ItemId);
            if (cart.Discount == 0)
            {
                total += item.Price * cart.Quantity;
            }
            else
            {
                total += item.Price * cart.Quantity * (100 - cart.Discount) / 100;
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
