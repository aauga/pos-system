using Domain.Common;

namespace Domain.Entities;

public class Cart : BaseEntity
{
    public Order Order { get; set; }
    public int OrderId { get; set; }
    public Item Item { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public string Description { get; set; }
}
