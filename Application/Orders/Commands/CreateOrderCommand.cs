using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class CreateOrderCommand : IRequest<OrderDTO>
{
    //public int Id { get; init; }
    public int CustomerId { get; init; }
    public int EmployeeId { get; init; }
    public decimal Total { get; init; }
    public int Tip { get; init; }
    public string? Delivery { get; init; }
    public DateTime Date { get; init; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = new Order
        {
            Id = 0,
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
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var orderDTO = new OrderDTO
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            EmployeeId = entity.EmployeeId,
            Total = entity.Total,
            Tip = entity.Tip,
            Delivery = entity.Delivery,
            Date = entity.Date,
        };

        return orderDTO;
    }
}
