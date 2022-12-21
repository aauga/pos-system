using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts;

public record GetOrderCartsQuery (int orderId) : IRequest<IEnumerable<CartDTO>>;


public class GetOrderCartsQueryHandler : IRequestHandler<GetOrderCartsQuery, IEnumerable<CartDTO>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderCartsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CartDTO>> Handle(GetOrderCartsQuery request, CancellationToken cancellationToken)
    {
        var list = await _dbContext.Carts
            .Where(b => b.OrderId == request.orderId)
            .OrderBy(b => b.Id)
            .ToListAsync();
        var DTOList = new List<CartDTO>();
        foreach (var item in list)
        {
            DTOList.Add(new CartDTO
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ItemId= item.ItemId,
                Quantity= item.Quantity,
                Discount= item.Discount,
                Description= item.Description
            });
        }
        return DTOList;
    }
}