using Domain.Entities;

namespace Application.Orders;

public class OrderDto
{
    public OrderDto(Order order)
    {
        Id = order.Id;
        CustomerId = order.CustomerId;
        EmployeeId = order.EmployeeId;
        TenantId = order.TenantId;
        Total = order.Total;
        Tip = order.Tip;
        Delivery = order.Delivery;
        Date = order.Date;
    }

    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public int TenantId { get; set; }
    public decimal Total { get; set; }
    public decimal Tip { get; set; }
    public string? Delivery { get; set; }
    public DateTime Date { get; set; }
}
