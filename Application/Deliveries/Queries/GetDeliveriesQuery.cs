using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public class GetDeliveriesQuery : IRequest<IEnumerable<DeliveryDto>>
{
    public int offset { get; init; } = 0;
    public int limit { get; init; } = 20;
    //public int? TenantId { get; init; }
}

public class GetDeliveriesQueryHandler : IRequestHandler<GetDeliveriesQuery, IEnumerable<DeliveryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetDeliveriesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<DeliveryDto>> Handle(GetDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Deliveries
            .OrderBy(b => b.Id)
            .Skip(request.offset)
            .Take(request.limit)
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