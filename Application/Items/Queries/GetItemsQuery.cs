using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public record GetItemsQuery : IAuthorizedRequest<IEnumerable<ItemDto>>
{
    public Employee employee;
    public int TenantId;
    public int Offset { get; init; } = 0;
    public int Limit { get; init; } = 20;

    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        this.employee = employee;
        return await userService.CanAccessTenantAsync(employee, TenantId);
    }
}

public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, IEnumerable<ItemDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetItemsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Items
            .OrderBy(b => b.Id)
            .Where(b => b.TenantId == request.employee.TenantId)
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(item => new ItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Category = item.Category,
                Price = item.Price,
                Description = item.Description,
                Brand = item.Brand,
                Photo = item.Photo,
            })
            .ToListAsync();

        return list;
    }
}