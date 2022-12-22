using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record CreateOrderDeliveryCommand(int orderId, DeliveryBodyDto deliveryBodyDto) : IRequest<DeliveryDto>;


public class CreateOrderDeliveryCommandHandler : IRequestHandler<CreateOrderDeliveryCommand, DeliveryDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderDeliveryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeliveryDto> Handle(CreateOrderDeliveryCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var entity = new Delivery
        {
            OrderId = request.orderId,
            Address = request.deliveryBodyDto.Address,
            PostCode = request.deliveryBodyDto.PostCode,
            Details= request.deliveryBodyDto.Details
        };
        

        _dbContext.Deliveries.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var deliveryDto = new DeliveryDto
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Address = entity.Address,
            PostCode = entity.PostCode,
            Details = entity.Details
        };

        return deliveryDto;
    }
}
