using Domain.Common;

namespace Domain.Entities;

public class Payment : BaseEntity
{
    public Order Order { get; set; }
    public int OrderId { get;set; }
    public string? Provider { get; set; }
    public string? Status { get; set; }
}
