using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public Employee Employee { get; set; }
    public int EmployeeId { get; set; }
    public Customer Customer { get; set; }
    public int CustomerId { get; set; }
}
