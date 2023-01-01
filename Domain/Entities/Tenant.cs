using Domain.Common;

namespace Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; }
    public DateTime ActiveFrom { get; set; }
    public DateTime ActiveTo { get; set; }
    public IEnumerable<Employee> Employees { get; set; }
    public IEnumerable<Customer> Customers { get; set; }
}
