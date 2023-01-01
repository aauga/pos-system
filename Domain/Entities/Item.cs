using Domain.Common;

namespace Domain.Entities;

public class Item : BaseEntity
{
    public string Title { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public IEnumerable<Cart> Carts { get; set; }
    public string Description { get; set; }
    public string Brand { get; set; }
    public string Photo { get; set; }
}
