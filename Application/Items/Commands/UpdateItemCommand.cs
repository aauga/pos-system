using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public record UpdateItemCommand(int Id, ItemBodyDto Item) : IAuthorizedRequest<ItemDto>
{
    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        return await userService.CanManageItemAsync(employee, Id);
    }
}

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

        entity.Title = request.Item.Title;
        entity.Category = request.Item.Category;
        entity.Price = request.Item.Price;
        entity.Description = request.Item.Description;
        entity.Brand = request.Item.Brand;
        entity.Photo = request.Item.Photo;

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
        };

        return itemDto;
    }
}
