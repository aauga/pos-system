using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Orders;

public record GetOrderQuery(int id) : IAuthorizedRequest<OrderDto>
{
    internal Employee Employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        Employee = employee;
        return await userService.CanManageOrdersAsync(employee);
    }
}

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.Employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.id);

        if (order == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.id);
        }

        var orderDto = new OrderDto(order);

        return orderDto;
    }
}