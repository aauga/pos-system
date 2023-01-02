using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.UpdateOrderCommand;

public record UpdateOrderCommand (int Id, OrderBodyDto orderBodyDto) : IAuthorizedRequest<OrderDto>
{
    public Employee Employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        Employee = employee;
        var order = await dbContext.Orders.FindAsync(Id);
        return order != null ? await userService.CanAccessTenantAsync(employee, order.TenantId) : true;
    }
}

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Orders.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        entity.CustomerId = request.orderBodyDto.CustomerId;
        entity.TenantId = request.Employee.TenantId;
        entity.EmployeeId = request.Employee.Id;
        entity.Total = request.orderBodyDto.Total;
        entity.Tip = request.orderBodyDto.Tip;
        entity.Delivery = request.orderBodyDto.Delivery;
        entity.Date = request.orderBodyDto.Date;

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
