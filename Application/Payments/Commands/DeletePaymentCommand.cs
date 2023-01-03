using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Payments;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments;

public record DeletePaymentCommand(int Id) : IRequest;

public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeletePaymentCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Payments.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Payment), request.Id);
        }
        _dbContext.Payments.Remove(entity);

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
