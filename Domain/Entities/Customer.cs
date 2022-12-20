using Domain.Common;

namespace Domain.Entities;

public class Customer : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public Tenant Tenant { get; set; }
    public int TenantId { get; set; }
    public IEnumerable<Order> Orders { get; set; }
}
