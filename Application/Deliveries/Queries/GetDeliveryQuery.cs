using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Deliveries;

public record GetDeliveryQuery(int id) : IRequest<DeliveryDto>;

public class GetDeliveryQueryHandler : IRequestHandler<GetDeliveryQuery, DeliveryDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetDeliveryQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeliveryDto> Handle(GetDeliveryQuery request, CancellationToken cancellationToken)
    {
        var delivery = await _dbContext.Deliveries.FindAsync(request.id);

        if (delivery == null)
        {
            throw new NotFoundException(nameof(Payment), request.id);
        }

        var deliveryDto = new DeliveryDto
        {
            Id = delivery.Id,
            OrderId = delivery.OrderId,
            Address = delivery.Address,
            PostCode = delivery.PostCode,
            Details = delivery.Details,
        };

        return deliveryDto;
    }
}