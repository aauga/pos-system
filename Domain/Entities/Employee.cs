﻿using Domain.Common;

namespace Domain.Entities;

public class Employee : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PersonalCode { get; set; }
    public string? Email { get; set; }
    public int TenantId { get; set; }
    public int PositionId { get; set; }
    public Order Order { get; set; }
}