using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public record DeleteItemCommand(int Id) : IAuthorizedRequest
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        return await userService.CanManageItemAsync(employee, Id);
    }
}

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteItemCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Items.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }
        _dbContext.Items.Remove(entity);

        await _dbContext.SaveChangesAsync();

        return Unit.Value;
    }
}
