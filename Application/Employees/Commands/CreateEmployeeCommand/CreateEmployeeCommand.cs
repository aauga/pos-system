using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Employees.Dtos;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Employees.Commands.CreateEmployeeCommand;

public record CreateEmployeeCommand : IAuthorizedRequest<EmployeeDto>
{
    public int TenantId { get; init; }

    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }

    public string Username { get; init; }
    public string Password { get; init; }
    public PositionType Position { get; init; }

    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        return await userService.CanManageTenantAsync(employee, TenantId);
    }
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateEmployeeCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (UsernameExists(request.Username))
        {
            throw new ValidationException(nameof(request.Username), "Username already exists");
        }

        if (request.Position == PositionType.Admin)
        {
            throw new ValidationException(nameof(request.Position), "You are trying to give forbidden privileges");
        }

        var employee = CreateEmployee(request);

        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();

        return new EmployeeDto(employee);
    }

    private Employee CreateEmployee(CreateEmployeeCommand request)
    {
        var id = CreateId();

        return new Employee
        {
            Id = id,
            TenantId = request.TenantId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Username = request.Username,
            Password = request.Password,
            Position = request.Position
        };
    }

    private int CreateId()
    {
        if (!_dbContext.Employees.Any())
        {
            return 1;
        }

        return _dbContext.Employees.Max(x => x.Id + 1);
    }

    private bool UsernameExists(string username)
    {
        return _dbContext.Employees.Any(x => x.Username == username);
    }
}