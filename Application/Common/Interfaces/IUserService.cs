using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IUserService
{
    Task<bool> CanAccessTenantAsync(Employee employee, int tenantId);
    Task<bool> CanManageTenantAsync(Employee employee, int tenantId);
    Task<bool> CanViewItemsAsync(Employee employee, int itemId);
    Task<bool> CanViewItemsAsync(Employee employee);
    Task<bool> CanCreateItemAsync(Employee employee);
    Task<bool> CanManageItemAsync(Employee employee, int itemId);
}