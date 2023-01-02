using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Payments;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments;

public class UpdateOrderPaymentCommand : IRequest<PaymentDto>
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string Provider { get; init; }
    public string Status { get; init; }
}

public class UpdateOrderPaymentCommandHandler : IRequestHandler<UpdateOrderPaymentCommand, PaymentDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderPaymentCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentDto> Handle(UpdateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Payments.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Payment), request.Id);
        }

        entity.OrderId = request.OrderId;
        entity.Provider = request.Provider;
        entity.Status = request.Status;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var paymentDto = new PaymentDto
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Provider = entity.Provider,
            Status = entity.Status,
        };

        return paymentDto;
    }
}
