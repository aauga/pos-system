using Domain.Common;

namespace Domain.Entities;

public class Tenant : BaseEntity
{
    public string? Name { get; set; }
    public DateTime ActiveFrom { get; set; }
    public DateTime ActiveTo { get; set; }
    public Order Order { get; set; }
}
