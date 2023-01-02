using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Payments.Commands.CreateOrderPaymentCommand;

public record CreateOrderPaymentCommand(int OrderId, PaymentBodyDto paymentBodyDto) : IAuthorizedRequest<PaymentDto>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var order = await dbContext.Orders.FindAsync(OrderId);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}


public class CreateOrderPaymentCommandHandler : IRequestHandler<CreateOrderPaymentCommand, PaymentDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderPaymentCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentDto> Handle(CreateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var entity = new Payment
        {
            OrderId = request.OrderId,
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
