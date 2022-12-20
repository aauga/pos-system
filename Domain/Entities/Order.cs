using Domain.Common;

namespace Domain.Entities;

public class Order : BaseEntity
{
    public Customer Customer { get; set; }
    public int CustomerId { get; set; }
    public Employee Employee { get; set; }
    public int EmployeeId { get; set; }
    public Tenant Tenant { get; set; }
    public int TenantId { get; set; }
    public decimal Total { get; set; }
    public decimal Tip { get; set; }
    public string? Delivery { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<Cart> Carts { get; set; }
    public IEnumerable<Payment> Payments { get; set; }
    public IEnumerable<Delivery> Deliveries { get; set; }
}
