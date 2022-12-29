using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.UpdateOrderCommand;

public record UpdateOrderCommand (int id, OrderBodyDto orderBodyDto) : IRequest<OrderDto>;


public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Orders.FindAsync(request.id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.id);
        }

        entity.CustomerId = request.orderBodyDto.CustomerId;
        entity.EmployeeId = request.orderBodyDto.EmployeeId;
        entity.TenantId = request.orderBodyDto.TenantId;
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
