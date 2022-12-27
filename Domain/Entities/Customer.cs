using Domain.Common;

namespace Domain.Entities;

public class Customer : BaseEntity
{
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public IEnumerable<Order> Orders { get; set; }

    #nullable enable
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
