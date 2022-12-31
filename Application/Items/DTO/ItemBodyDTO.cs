using Domain.Entities;

namespace Application.Items;

public record ItemBodyDto
{
    public string Title { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Brand { get; set; }
    public string Photo { get; set; }
}
