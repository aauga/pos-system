using Domain.Common;

namespace Domain.Entities;

public class Delivery : BaseEntity
{
    public Order Order { get; set; }
    public int OrderId { get; set; }
    public string Address { get; set; }
    public string PostCode { get; set; }

    #nullable enable
    public string? Details { get; set; }
}
