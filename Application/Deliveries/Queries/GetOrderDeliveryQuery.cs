using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record GetOrderDeliveryQuery(int OrderId) : IAuthorizedRequest<IEnumerable<DeliveryDto>>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
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
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var list = await _dbContext.Deliveries
            .Where(b => b.OrderId == request.OrderId)
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