using Domain.Entities;

namespace Application.Customers.Dtos;

public record CustomerDto
{
    public CustomerDto(Customer customer)
    {
        Id = customer.Id;
        FirstName = customer.FirstName;
        LastName = customer.LastName;
        Email = customer.Email;
        PhoneNumber = customer.PhoneNumber;
        TenantId = customer.TenantId;
    }

    public int Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public int TenantId { get; init; }
}