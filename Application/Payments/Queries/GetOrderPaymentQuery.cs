using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments;

public record GetOrderPaymentQuery(int orderId) : IAuthorizedRequest<IEnumerable<PaymentDto>>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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
        var ten = request.employee.TenantId;
        var ord = request.orderId;
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.orderId);

        if (order == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var list = await _dbContext.Payments
            .Where(b => b.OrderId == request.orderId)
            .Select(payment => new PaymentDto(payment))
            .ToListAsync();

        return list;
    }
}