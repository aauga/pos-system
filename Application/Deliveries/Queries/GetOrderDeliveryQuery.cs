using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record GetOrderDeliveryQuery (int orderId) : IRequest<IEnumerable<DeliveryDTO>>;


public class GetOrderDeliveryQueryHandler : IRequestHandler<GetOrderDeliveryQuery, IEnumerable<DeliveryDTO>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderDeliveryQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<DeliveryDTO>> Handle(GetOrderDeliveryQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var list = await _dbContext.Deliveries
            .Where(b => b.OrderId == request.orderId)
            .ToListAsync();

        var DTOList = new List<DeliveryDTO>();
        foreach (var item in list)
        {
            DTOList.Add(new DeliveryDTO
            {
                Id = item.Id,
                OrderId= item.OrderId,
                Address= item.Address,
                PostCode= item.PostCode,
                Details= item.Details
            });
        }
        return DTOList;
    }
}