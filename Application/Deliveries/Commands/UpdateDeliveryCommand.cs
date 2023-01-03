using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public class UpdateDeliveryCommand : IRequest<DeliveryDto>
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string Address { get; init; }
    public string PostCode { get; init; }
    public string Details { get; init; }
}

public class UpdateDeliveryCommandHandler : IRequestHandler<UpdateDeliveryCommand, DeliveryDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateDeliveryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeliveryDto> Handle(UpdateDeliveryCommand request, CancellationToken cancellationToken)
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
