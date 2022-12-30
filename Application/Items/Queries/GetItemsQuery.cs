using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Items;

public record GetItemsQuery : IRequest<IEnumerable<ItemDto>>
{
    public int offset { get; init; } = 0;
    public int limit { get; init; } = 20;
}

public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, IEnumerable<ItemDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetItemsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // TODO: add filtering by tenantId after merging auth
    public async Task<IEnumerable<ItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Items
            .OrderBy(b => b.Id)
            .Skip(request.offset)
            .Take(request.limit)
            .Select(item => new ItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Category = item.Category,
                Price = item.Price,
                Description = item.Description,
                Brand = item.Brand,
                Photo = item.Photo,
                TenantId = item.TenantId
            })
            .ToListAsync();

        return list;
    }
}