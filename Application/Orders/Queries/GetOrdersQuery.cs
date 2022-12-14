using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public record GetOrdersQuery : IAuthorizedRequest<IEnumerable<OrderDto>>
{
    public int offset { get; init; } = 0;
    public int limit { get; init; } = 20;
    public Employee Employee;
    public int TenantId;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        Employee = employee;
        return await userService.CanAccessTenantAsync(employee, TenantId);
    }
}

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrdersQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Orders
            .OrderBy(b => b.Id)
            .Where(b => b.TenantId == request.Employee.TenantId)
            .Skip(request.offset)
            .Take(request.limit)
            .Select(order => new OrderDto(order))
            .ToListAsync();
       
        return list;
    }
}