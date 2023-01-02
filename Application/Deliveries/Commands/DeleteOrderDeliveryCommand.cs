using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Deliveries;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record DeleteOrderDeliveryCommand(int Id) : IRequest;

public class DeleteOrderDeliveryCommandHandler : IRequestHandler<DeleteOrderDeliveryCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteOrderDeliveryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteOrderDeliveryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Deliveries.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Delivery), request.Id);
        }
        _dbContext.Deliveries.Remove(entity);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        return Unit.Value;
    }
}
