using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Payments;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments;

public class UpdatePaymentCommand : IRequest<PaymentDto>
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string Provider { get; init; }
    public string Status { get; init; }
}

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, PaymentDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdatePaymentCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentDto> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _dbContext.Payments.FindAsync(request.Id);

        if (payment == null)
        {
            throw new NotFoundException(nameof(Payment), request.Id);
        }

        payment.OrderId = request.OrderId;
        payment.Provider = request.Provider;
        payment.Status = request.Status;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var paymentDto = new PaymentDto(payment);

        return paymentDto;
    }
}
