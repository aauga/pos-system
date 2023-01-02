using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Payments;

public record GetPaymentQuery(int id) : IRequest<PaymentDto>;

public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, PaymentDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetPaymentQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentDto> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _dbContext.Payments.FindAsync(request.id);

        if (payment == null)
        {
            throw new NotFoundException(nameof(Payment), request.id);
        }

        var paymentDto = new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Provider = payment.Provider,
            Status = payment.Status
        };

        return paymentDto;
    }
}