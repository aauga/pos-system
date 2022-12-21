namespace Application.Carts;

public class CartBodyDTO
{
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public string? Description { get; set; }
}
