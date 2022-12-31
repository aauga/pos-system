using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class UpdateOrderCommand : IRequest<OrderDto>
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public int EmployeeId { get; init; }
    public decimal Total { get; init; }
    public int Tip { get; init; }
    public string Delivery { get; init; }
    public DateTime Date { get; init; }
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

        entity.CustomerId = request.CustomerId;
        entity.EmployeeId = request.EmployeeId;
        entity.Total = request.Total;
        entity.Tip = request.Tip;
        entity.Delivery = request.Delivery;
        entity.Date = request.Date;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var orderDto = new OrderDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            EmployeeId = entity.EmployeeId,
            Total = entity.Total,
            Tip = entity.Tip,
            Delivery = entity.Delivery,
            Date = entity.Date
        };

        return orderDto;
    }
}
