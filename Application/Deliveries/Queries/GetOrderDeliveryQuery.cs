using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record GetOrderDeliveryQuery (int orderId) : IAuthorizedRequest<IEnumerable<DeliveryDto>>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
    }
}


public class GetOrderDeliveryQueryHandler : IRequestHandler<GetOrderDeliveryQuery, IEnumerable<DeliveryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderDeliveryQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<DeliveryDto>> Handle(GetOrderDeliveryQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.orderId);

        if (order == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var list = await _dbContext.Deliveries
            .Where(b => b.OrderId == request.orderId)
            .Select(item => new DeliveryDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                Address = item.Address,
                PostCode = item.PostCode,
                Details = item.Details
            })
            .ToListAsync();

        return list;
    }
}