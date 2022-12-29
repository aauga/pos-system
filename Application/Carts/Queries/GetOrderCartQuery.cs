using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts;

public record GetOrderCartQuery (int orderId, int itemId) : IRequest<CartDto>;


public class GetOrderCartQueryHandler : IRequestHandler<GetOrderCartQuery, CartDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderCartQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartDto> Handle(GetOrderCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts
            .SingleAsync(b => b.OrderId == request.orderId && b.ItemId == request.itemId);

        if (cart == null)
        {
            throw new NotFoundException(nameof(Cart));
        }

        var cartDto = new CartDto(cart);

        return cartDto;
    }
}