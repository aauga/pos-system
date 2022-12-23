using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record GetOrderDeliveryQuery (int orderId) : IRequest<IEnumerable<DeliveryDto>>;


public class GetOrderDeliveryQueryHandler : IRequestHandler<GetOrderDeliveryQuery, IEnumerable<DeliveryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderDeliveryQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<DeliveryDto>> Handle(GetOrderDeliveryQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
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