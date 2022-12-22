using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments;

public record GetOrderPaymentQuery (int orderId) : IRequest<IEnumerable<PaymentDTO>>;


public class GetOrderPaymentQueryHandler : IRequestHandler<GetOrderPaymentQuery, IEnumerable<PaymentDTO>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderPaymentQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PaymentDTO>> Handle(GetOrderPaymentQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var list = await _dbContext.Payments
            .Where(b => b.OrderId == request.orderId)
            .Select(item => new PaymentDTO
            {
                Id = item.Id,
                OrderId = item.OrderId,
                Provider = item.Provider,
                Status = item.Status
            })
            .ToListAsync();

        return list;
    }
}