namespace Application.Orders;

public class OrderDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public decimal Total { get; set; }
    public decimal Tip { get; set; }
    public string? Delivery { get; set; }
    public DateTime Date { get; set; }
}
