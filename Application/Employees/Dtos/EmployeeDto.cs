using Domain.Entities;
using Domain.Enums;

namespace Application.Employees.Dtos;

public record EmployeeDto : BaseDto
{
    public EmployeeDto(Employee employee)
    {
        Id = employee.Id;
        TenantId = employee.TenantId;
        FirstName = employee.FirstName;
        LastName = employee.LastName;
        Email = employee.Email;
        Username = employee.Username;
        Password = employee.Password;
        Position = employee.Position;
    }

    public int? TenantId { get; init; }

    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }

    public string Username { get; init; }
    public string Password { get; init; }
    public PositionType Position { get; init; }
}