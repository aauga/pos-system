using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Payments.Commands.CreateOrderPaymentCommand;

public record CreateOrderPaymentCommand(int orderId, PaymentBodyDto paymentBodyDto) : IRequest<PaymentDto>;


public class CreateOrderPaymentCommandHandler : IRequestHandler<CreateOrderPaymentCommand, PaymentDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderPaymentCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentDto> Handle(CreateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var entity = new Payment
        {
            OrderId = request.orderId,
            Provider = request.paymentBodyDto.Provider,
            Status = request.paymentBodyDto.Status
        };


        _dbContext.Payments.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var deliveryDto = new PaymentDto(entity);

        return deliveryDto;
    }
}
