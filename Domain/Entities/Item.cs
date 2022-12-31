using Domain.Common;

namespace Domain.Entities;

public class Item : BaseEntity
{
    public string Title { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }

    #nullable enable
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Photo { get; set; }
    public IEnumerable<Cart> Carts { get; set; }
    public IEnumerable<Review> Reviews { get; set; }
    public Tenant Tenant { get; set; }
    public int TenantId { get; set; }
}
