using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public record CreateItemCommand : IAuthorizedRequest<ItemDto>
{
    public Employee Employee;
    public int TenantId;
    public string Title { get; init; }
    public string Category { get; init; }
    public decimal Price { get; init; }
    public string Description { get; init; }
    public string Brand { get; init; }
    public string Photo { get; init; }

    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.Employee = employee;
        return await userService.CanManageTenantAsync(employee, TenantId);
    }
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
            TenantId = request.Employee.TenantId,
        };

        _dbContext.Items.Add(entity);

        await _dbContext.SaveChangesAsync();

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