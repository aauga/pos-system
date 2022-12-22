using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class GetOrdersQuery : IRequest<IEnumerable<OrderDTO>>
{
    public int offset { get; init; } = 0;
    public int limit { get; init; } = 20;
    //public int? TenantId { get; init; }
}

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDTO>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrdersQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<OrderDTO>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Orders
            .OrderBy(b => b.Id)
            .Skip(request.offset)
            .Take(request.limit)
            .Select(item => new OrderDTO
            {
                Id = item.Id,
                CustomerId = item.CustomerId,
                EmployeeId = item.EmployeeId,
                Total = item.Total,
                Tip = item.Tip,
                Delivery = item.Delivery,
                Date = item.Date
            })
            .ToListAsync();
       
        return list;
    }
}