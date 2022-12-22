using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts;

public record GetOrderCartQuery (int orderId, int itemId) : IRequest<CartDTO>;


public class GetOrderCartQueryHandler : IRequestHandler<GetOrderCartQuery, CartDTO>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderCartQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDTO> Handle(GetOrderCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts
            .SingleAsync(b => b.OrderId == request.orderId && b.ItemId == request.itemId);

        if (cart == null)
        {
            throw new NotFoundException(nameof(Cart));
        }

        var cartDTO = new CartDTO
        {
            Id = cart.Id,
            OrderId = request.orderId,
            ItemId = request.itemId,
            Quantity = cart.Quantity,
            Discount = cart.Discount,
            Description = cart.Description
        };

        return cartDTO;
    }
}