﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Orders;

public record GetOrderQuery(int Id) : IRequest<OrderDTO>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDTO> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.Id) ??
            throw new NotFoundException(nameof(Order), request.Id);

        var orderDTO = new OrderDTO
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            EmployeeId = order.EmployeeId,
            Total = order.Total,
            Tip = order.Tip,
            Delivery = order.Delivery,
            Date = order.Date
        };

        return orderDTO;
    }
}