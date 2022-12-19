using Domain.Common;

namespace Domain.Entities;

public class Item : BaseEntity
{
    public Order Order { get; set; }
    public int OrderId { get; set; }
    public string? Title { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Photo { get; set; }
}
