using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.CreateOrderCommand;

public record CreateOrderCommand(OrderBodyDto orderBodyDto) : IAuthorizedRequest<OrderDto>
{
    internal Employee employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanManageOrdersAsync(employee);
    }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = new Order
        {
            CustomerId = request.orderBodyDto.CustomerId,
            EmployeeId = request.employee.Id,
            TenantId = request.employee.TenantId,
            Total = request.orderBodyDto.Total,
            Tip = request.orderBodyDto.Tip,
            Delivery = request.orderBodyDto.Delivery,
            Date = request.orderBodyDto.Date
        };

        _dbContext.Orders.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var orderDto = new OrderDto(entity);

        return orderDto;
    }
}
