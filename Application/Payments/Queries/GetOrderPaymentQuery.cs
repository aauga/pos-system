using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments;

public record GetOrderPaymentQuery(int OrderId) : IAuthorizedRequest<IEnumerable<PaymentDto>>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}

public class GetOrderPaymentQueryHandler : IRequestHandler<GetOrderPaymentQuery, IEnumerable<PaymentDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderPaymentQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetOrderPaymentQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var list = await _dbContext.Payments
            .Where(b => b.OrderId == request.OrderId)
            .Select(payment => new PaymentDto(payment))
            .ToListAsync();

        return list;
    }
}