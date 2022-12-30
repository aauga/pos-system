using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public class CreateItemCommand : IRequest<ItemDto>
{
    public string? Title { get; init; }
    public string? Category { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public string? Brand { get; init; }
    public string? Photo { get; init; }
    public int TenantId { get; init; }
}

public class CreateItemCommandHandler: IRequestHandler<CreateItemCommand, ItemDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateItemCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new Item
        {
            Title = request.Title,
            Category = request.Category,
            Price = request.Price,
            Description = request.Description,
            Brand = request.Brand,
            Photo = request.Photo,
            TenantId = request.TenantId,
        };

        _dbContext.Items.Add(entity);

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