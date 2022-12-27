using Domain.Entities;
using MediatR;

namespace Application.Common.Interfaces;

internal interface IAuthorizedRequest<out TResponse> : IRequest<TResponse>
{
    Task<bool> Authorize(
        Employee employee,
        IUserService userService,
        IApplicationDbContext dbContext
    );
}

internal interface IAuthorizedRequest : IRequest
{
    Task<bool> Authorize(
        Employee employee,
        IUserService userService,
        IApplicationDbContext dbContext
    );
}