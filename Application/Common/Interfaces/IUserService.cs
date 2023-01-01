using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IUserService
{
    Task<bool> CanAccessTenantAsync(Employee employee, int tenantId);
    Task<bool> CanManageTenantAsync(Employee employee, int tenantId);
}