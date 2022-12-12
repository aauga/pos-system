using Domain.Common;

namespace Domain.Entities;

public class Employee : BaseEntity
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string PersonalCode { get; set; }
}