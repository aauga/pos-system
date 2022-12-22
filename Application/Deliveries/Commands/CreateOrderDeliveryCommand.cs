using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record CreateOrderDeliveryCommand(int orderId, DeliveryBodyDTO deliveryBodyDTO) : IRequest<DeliveryDTO>;


public class CreateOrderDeliveryCommandHandler : IRequestHandler<CreateOrderDeliveryCommand, DeliveryDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderDeliveryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeliveryDTO> Handle(CreateOrderDeliveryCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.orderId);

        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.orderId);
        }

        var entity = new Delivery
        {
            OrderId = request.orderId,
            Address = request.deliveryBodyDTO.Address,
            PostCode = request.deliveryBodyDTO.PostCode,
            Details= request.deliveryBodyDTO.Details
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

        var deliveryDTO = new DeliveryDTO
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Address = entity.Address,
            PostCode = entity.PostCode,
            Details = entity.Details
        };

        return deliveryDTO;
    }
}
