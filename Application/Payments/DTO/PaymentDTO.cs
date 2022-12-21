namespace Application.Payments;

public class PaymentDTO
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? Provider { get; set; }
    public string? Status { get; set; }
}
