namespace Application.Carts;

public class CartDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public string? Description { get; set; }
}
