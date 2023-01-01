using Domain.Common;

namespace Domain.Entities;

public class Review : BaseEntity
{
    public Item Item { get; set; }
    public int ItemId { get; set; }
    public Employee Employee { get; set; }
    public int EmployeeId { get; set; }
    public int Rating { get; set; }
    public String Description { get; set; }
    public String Photo { get; set; }
}
