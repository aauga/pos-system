using Domain.Entities;

namespace Application.Tenants.Dtos;

public record TenantDto : BaseDto
{
    public TenantDto(Tenant tenant)
    {
        Id = tenant.Id;
        Name = tenant.Name;
        ActiveFrom = tenant.ActiveFrom;
        ActiveTo = tenant.ActiveTo;
    }
    
    public string Name { get; init; }
    public DateTime ActiveFrom { get; init; }
    public DateTime ActiveTo { get; init; }
}