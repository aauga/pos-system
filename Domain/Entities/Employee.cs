using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }
    public PositionType Position { get; set; }

    public IEnumerable<Order> Orders { get; set; }
}