using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments;

public class GetPaymentsQuery : IRequest<IEnumerable<PaymentDto>>
{
    public int offset { get; init; } = 0;
    public int limit { get; init; } = 20;
    //public int? TenantId { get; init; }
}

public class GetOrdersQueryHandler : IRequestHandler<GetPaymentsQuery, IEnumerable<PaymentDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrdersQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Payments
            .OrderBy(b => b.Id)
            .Skip(request.offset)
            .Take(request.limit)
            .Select(item => new PaymentDto(item))
            .ToListAsync();

        return list;
    }
}