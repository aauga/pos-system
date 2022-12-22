using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Payments;

public record CreateOrderPaymentCommand (int orderId, PaymentBodyDTO paymentBodyDTO) : IRequest<PaymentDTO>;


public class CreateOrderPaymentCommandHandler : IRequestHandler<CreateOrderPaymentCommand, PaymentDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderPaymentCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentDTO> Handle(CreateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var entity = new Payment
        {
            OrderId = request.orderId,
            Provider = request.paymentBodyDTO.Provider,
            Status= request.paymentBodyDTO.Status
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

        var deliveryDTO = new PaymentDTO
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Provider = request.paymentBodyDTO.Provider,
            Status = request.paymentBodyDTO.Status
        };

        return deliveryDTO;
    }
}
