using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthorizationBehaviour<TRequest, TResponse>> _logger;

    public AuthorizationBehaviour(
        IApplicationDbContext dbContext,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthorizationBehaviour<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;

        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }
        
        // From here, user is authenticated with the correct username and password
        var username = GetUsername(user);
        var employee = await GetEmployeeAsync(username);
        var userAuthorized = await CheckIfUserAuthorized(employee, request);

        if (!userAuthorized)
        {
            throw new ForbiddenAccessException();
        }

        return await next();
    }

    private string GetUsername(ClaimsPrincipal user)
    {
        var username = user.Claims.FirstOrDefault(x => x.Type == "Username").Value;

        if (string.IsNullOrEmpty(username))
        {
            throw new ForbiddenAccessException();
        }

        return username;
    }

    private async Task<Employee> GetEmployeeAsync(string username)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Username == username);

        if (employee is null)
        {
            throw new ForbiddenAccessException();
        }

        return employee;
    }

    private async Task<bool> CheckIfUserAuthorized(Employee employee, TRequest request)
    {
        if (request is IAuthorizedRequest<TResponse>)
        {
            var authorizedRequest = request as IAuthorizedRequest<TResponse>;
            return await authorizedRequest.Authorize(employee, _userService, _dbContext);
        }

        if (request is IAuthorizedRequest)
        {
            var authorizedRequest = request as IAuthorizedRequest;
            return await authorizedRequest.Authorize(employee, _userService, _dbContext);
        }

        return true;
    }
}