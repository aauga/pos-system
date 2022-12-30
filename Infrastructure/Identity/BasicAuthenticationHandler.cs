using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    private readonly IApplicationDbContext _dbContext;
    
    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApplicationDbContext dbContext)
        : base(options, logger, encoder, clock)
    {
        _dbContext = dbContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Validate authorization header
        string authorizationHeader = Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return AuthenticateResult.NoResult();
        }

        if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("User does not use Basic authentication");
        }

        // Validate token
        string token = authorizationHeader.Substring("Basic ".Length).Trim();

        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("Credentials not provided");
        }

        // Get credentials
        try
        {
            var usernameAndPassword = GetUsernameAndPasswordFromToken(token);
            var employee = await GetEmployeeAsync(usernameAndPassword);
            var ticket = GenerateTicket(employee);

            return AuthenticateResult.Success(ticket);
        }
        catch (System.Exception)
        {
            return AuthenticateResult.Fail("Error authenticating user");
            throw;
        }
    }

    private Tuple<string, string> GetUsernameAndPasswordFromToken(string token)
    {
        var encodedBase64 = token.Replace("Basic ", "");
        var bytes = Convert.FromBase64String(encodedBase64);
        var decodedBase64 = Encoding.UTF8.GetString(bytes);

        var credentials = decodedBase64.Split(":");
        return new Tuple<string, string>(credentials[0], credentials[1]);
    }

    private async Task<Employee> GetEmployeeAsync(Tuple<string, string> credentials)
    {
        return await _dbContext.Employees
            .Where(x => x.Username == credentials.Item1)
            .Where(x => x.Password == credentials.Item2)
            .SingleAsync();
    }

    private AuthenticationTicket GenerateTicket(Employee employee)
    {
        var claims = new[] {
            new Claim("EmployeeId", employee.Id.ToString()),
            new Claim("Username", employee.Username),
            new Claim("TenantId", employee.TenantId.ToString())
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationTicket(principal, Scheme.Name);
    }
}
