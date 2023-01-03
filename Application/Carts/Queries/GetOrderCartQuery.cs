using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts;

public record GetOrderCartQuery (int OrderId, int ItemId) : IAuthorizedRequest<CartDto>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}

public class GetOrderCartQueryHandler : IRequestHandler<GetOrderCartQuery, CartDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderCartQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDto> Handle(GetOrderCartQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var cart = await _dbContext.Carts
            .SingleAsync(b => b.OrderId == request.OrderId && b.ItemId == request.ItemId);

        if (cart == null)
        {
            throw new NotFoundException(nameof(Cart));
        }

        var cartDto = new CartDto(cart);

        return cartDto;
    }
}