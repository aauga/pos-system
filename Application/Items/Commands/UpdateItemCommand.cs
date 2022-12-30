using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public record UpdateItemCommand (int Id, ItemBodyDto item) : IRequest<ItemDto>;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, ItemDto>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateItemCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Items.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        entity.Title = request.item.Title;
        entity.Category = request.item.Category;
        entity.Price = request.item.Price;
        entity.Description = request.item.Description;
        entity.Brand = request.item.Brand;
        entity.Photo = request.item.Photo;
        entity.TenantId = request.item.TenantId;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenAccessException();
        }

        var itemDto = new ItemDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Category = entity.Category,
            Price = entity.Price,
            Description = entity.Description,
            Brand = entity.Brand,
            Photo = entity.Photo,
            TenantId = entity.TenantId
        };

        return itemDto;
    }
}
