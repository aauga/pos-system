using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Payments.Commands.CreateOrderPaymentCommand;

public record CreateOrderPaymentCommand(int orderId, PaymentBodyDto paymentBodyDto) : IAuthorizedRequest<PaymentDto>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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
        var order = await _dbContext.Orders.Where(b => b.TenantId == request.employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.orderId);

        if (order == default(Order))
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
