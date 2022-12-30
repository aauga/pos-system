using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts;

public record GetOrderCartQuery (int orderId, int itemId) : IAuthorizedRequest<CartDto>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.orderId);

        if (order == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var cart = await _dbContext.Carts
            .SingleAsync(b => b.OrderId == request.orderId && b.ItemId == request.itemId);

        if (cart == null)
        {
            throw new NotFoundException(nameof(Cart));
        }

        var cartDto = new CartDto(cart);

        return cartDto;
    }
}