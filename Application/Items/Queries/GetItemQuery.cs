using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public record GetItemQuery(int Id) : IAuthorizedRequest<ItemDto>
{
    public Employee Employee;

    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.Employee = employee;
        var item = await dbContext.Items.FindAsync(Id);
        return item != null ? await userService.CanAccessTenantAsync(employee, item.TenantId) : true;
    }
}

public class GetItemQueryHandler : IRequestHandler<GetItemQuery, ItemDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetItemQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ItemDto> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.FindAsync(request.Id);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        var itemDto = new ItemDto
        {
            Id = item.Id,
            Title = item.Title,
            Category = item.Category,
            Price = item.Price,
            Description = item.Description,
            Brand = item.Brand,
            Photo = item.Photo,
        };

        return itemDto;
    }
}