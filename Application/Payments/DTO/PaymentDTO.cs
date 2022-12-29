using Domain.Entities;

namespace Application.Payments;

public class PaymentDto
{
    public PaymentDto(Payment payment)
    {
        Id = payment.Id;
        OrderId = payment.OrderId;
        Provider = payment.Provider;
        Status = payment.Status;
    } 
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? Provider { get; set; }
    public string? Status { get; set; }
}
