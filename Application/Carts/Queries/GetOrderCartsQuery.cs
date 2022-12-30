using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts;

public record GetOrderCartsQuery (int orderId) : IAuthorizedRequest<IEnumerable<CartDto>>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.orderId);

        if (order == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var list = await _dbContext.Carts
            .Where(b => b.OrderId == request.orderId)
            .OrderBy(b => b.Id)
            .Select(cart => new CartDto(cart))
            .ToListAsync();

        return list;
    }
}