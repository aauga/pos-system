using Domain.Entities;

namespace Application.Orders;

public class OrderBodyDto
{
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public decimal Tip { get; set; }
    public string? Delivery { get; set; }
    public DateTime Date { get; set; }
}
