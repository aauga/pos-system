using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.UpdateOrderCommand;

public record UpdateOrderCommand (int id, OrderBodyDto orderBodyDto) : IAuthorizedRequest<OrderDto>
{
    internal Employee Employee;
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        Employee = employee;
        return await userService.CanManageOrdersAsync(employee);
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
        var entity = await _dbContext.Orders.Where(b => b.TenantId == request.Employee.TenantId).SingleOrDefaultAsync(b => b.Id == request.id);

        if (entity == default(Order))
        {
            throw new NotFoundException(nameof(Order), request.id);
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
