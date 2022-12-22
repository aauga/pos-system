using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class CreateOrderCommand : IRequest<OrderDto>
{
    //public int Id { get; init; }
    public int CustomerId { get; init; }
    public int EmployeeId { get; init; }
    public decimal Total { get; init; }
    public int Tip { get; init; }
    public string? Delivery { get; init; }
    public DateTime Date { get; init; }
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
            CustomerId = request.CustomerId,
            EmployeeId = request.EmployeeId,
            Total = request.Total,
            Tip = request.Tip,
            Delivery = request.Delivery,
            Date = request.Date
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

        var orderDto = new OrderDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            EmployeeId = entity.EmployeeId,
            Total = entity.Total,
            Tip = entity.Tip,
            Delivery = entity.Delivery,
            Date = entity.Date,
        };

        return orderDto;
    }
}
