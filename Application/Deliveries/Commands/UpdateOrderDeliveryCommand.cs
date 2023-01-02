using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Deliveries;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public class UpdateOrderDeliveryCommand : IRequest<DeliveryDto>
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string Address { get; init; }
    public string PostCode { get; init; }
    public string Details { get; init; }
}

public class UpdateOrderDeliveryCommandHandler : IRequestHandler<UpdateOrderDeliveryCommand, DeliveryDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateOrderDeliveryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeliveryDto> Handle(UpdateOrderDeliveryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Deliveries.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Delivery), request.Id);
        }

        entity.OrderId = request.OrderId;
        entity.Address = request.Address;
        entity.PostCode = request.PostCode;
        entity.Details = request.Details;

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
            Details = entity.Details,
        };

        return deliveryDto;
    }
}
