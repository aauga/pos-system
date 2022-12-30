using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Items;

public record GetItemQuery(int id) : IRequest<ItemDto>;

public class GetOrderQueryHandler : IRequestHandler<GetItemQuery, ItemDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ItemDto> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.FindAsync(request.id);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.id);
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
            TenantId = item.TenantId
        };

        return itemDto;
    }
}