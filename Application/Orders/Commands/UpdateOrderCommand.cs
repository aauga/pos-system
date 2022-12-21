﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class UpdateOrderCommand : IRequest<OrderDTO>
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public int EmployeeId { get; init; }
    public decimal Total { get; init; }
    public int Tip { get; init; }
    public string? Delivery { get; init; }
    public DateTime Date { get; init; }
}

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDTO> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Orders.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        //entity.Id = request.Id;
        entity.CustomerId = request.CustomerId;
        entity.EmployeeId = request.EmployeeId;
        entity.Total = request.Total;
        entity.Tip = request.Tip;
        entity.Delivery = request.Delivery;
        entity.Date = request.Date;

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
            Id = request.Id,
            CustomerId = request.CustomerId,
            EmployeeId = request.EmployeeId,
            Total = request.Total,
            Tip = request.Tip,
            Delivery = request.Delivery,
            Date = request.Date,
        };

        return orderDTO;
    }
}