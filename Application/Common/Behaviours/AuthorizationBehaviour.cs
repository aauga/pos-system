using System.Text;
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
        var credentials = GetUserCredentials();
        var employee = await GetEmployeeAsync(credentials);
        var userAuthorized = await UserAuthorized(employee, request);

        if (!userAuthorized)
        {
            throw new ForbiddenAccessException();
        }

        return await next();
    }

    private string[] GetUserCredentials()
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"][0];

        try
        {
            var base64 = token.Replace("Basic ", "");
            var bytes = Convert.FromBase64String(base64);
            var decodedValue = Encoding.UTF8.GetString(bytes);

            return decodedValue.Split(":");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while authorizing user");
            throw new ForbiddenAccessException();
        }
    }

    private async Task<Employee> GetEmployeeAsync(string[] credentials)
    {
        return await _dbContext.Employees.FirstOrDefaultAsync(x =>
            x.Username == credentials[0] &&
            x.Password == credentials[1]);
    }

    private async Task<bool> UserAuthorized(Employee employee, TRequest request)
    {
        if (request is IAuthorizedRequest<TResponse>)
        {
            var authorizedRequest = request as IAuthorizedRequest<TResponse>;
            return await authorizedRequest.Authorize(employee, _userService, _dbContext);
        }

        if (request is IAuthorizedRequest)
        {
            var authorizedRequest = request as IAuthorizedRequest<TResponse>;
            return await authorizedRequest.Authorize(employee, _userService, _dbContext);
        }

        return false;
    }
}