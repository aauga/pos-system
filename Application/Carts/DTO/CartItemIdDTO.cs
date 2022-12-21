namespace Application.Carts;

public class CartItemIdDTO
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public string? Description { get; set; }
}
