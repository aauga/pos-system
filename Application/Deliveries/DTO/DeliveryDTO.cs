namespace Application.Deliveries;

public class DeliveryDTO
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? Address { get; set; }
    public string? PostCode { get; set; }
    public string? Details { get; set; }
}
