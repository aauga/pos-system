using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts;

public record GetOrderCartsQuery (int OrderId) : IAuthorizedRequest<IEnumerable<CartDto>>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}

public class GetOrderCartsQueryHandler : IRequestHandler<GetOrderCartsQuery, IEnumerable<CartDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderCartsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CartDto>> Handle(GetOrderCartsQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var list = await _dbContext.Carts
            .Where(b => b.OrderId == request.OrderId)
            .OrderBy(b => b.Id)
            .Select(cart => new CartDto(cart))
            .ToListAsync();

        return list;
    }
}