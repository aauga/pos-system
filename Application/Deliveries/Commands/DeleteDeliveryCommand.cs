using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Deliveries;

public record DeleteDeliveryCommand(int Id) : IRequest;

public class DeleteDeliveryCommandHandler : IRequestHandler<DeleteDeliveryCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteDeliveryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteDeliveryCommand request, CancellationToken cancellationToken)
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
