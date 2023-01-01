using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Orders;

public record GetOrderQuery(int Id) : IAuthorizedRequest<OrderDto>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(Id);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
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
        var order = await _dbContext.Orders.FindAsync(request.Id);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        var orderDto = new OrderDto(order);

        return orderDto;
    }
}