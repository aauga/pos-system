using Domain.Entities;

namespace Application.Carts;

public class CartDto
{
    public CartDto(Cart cart)
    {
        Id = cart.Id;
        OrderId = cart.OrderId;
        ItemId = cart.ItemId;
        Quantity= cart.Quantity;
        Discount= cart.Discount;
        Description= cart.Description;
    }
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public string Description { get; set; }
}
