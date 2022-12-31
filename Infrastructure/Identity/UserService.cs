using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _dbContext;

    public UserService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CanAccessTenantAsync(Employee employee, int tenantId)
    {
        var tenantExists = await TenantExistsAsync(tenantId);

        if (!tenantExists)
        {
            return false;
        }
        
        if (employee.Position == PositionType.Admin)
        {
            return true;
        }

        var canAccessTenant = employee.TenantId == tenantId;

        return await Task.FromResult(canAccessTenant);
    }

    public async Task<bool> CanManageTenantAsync(Employee employee, int tenantId)
    {
        var tenantExists = await TenantExistsAsync(tenantId);

        if (!tenantExists)
        {
            return false;
        }

        if (employee.Position == PositionType.Admin)
        {
            return true;
        }

        var canManageTenant = employee.TenantId == tenantId && employee.Position == PositionType.Manager;

        return await Task.FromResult(canManageTenant);
    }

    private async Task<bool> TenantExistsAsync(int tenantId)
    {
        return await _dbContext.Tenants.AnyAsync(x => x.Id == tenantId);
    }

    public async Task<bool> CanViewItemsAsync(Employee employee, int itemId)
    {
        var item = await _dbContext.Items.FindAsync(itemId);
        var authorizedTenant = item != null ? item.TenantId == employee.TenantId : true;
        var authorizedPosition = employee.Position >= PositionType.Cashier;

        return await Task.FromResult(authorizedTenant && authorizedPosition);
    }

    public async Task<bool> CanViewItemsAsync(Employee employee)
    {
        var authorized = employee.Position >= PositionType.Cashier;

        return await Task.FromResult(authorized);
    }

    public async Task<bool> CanCreateItemAsync(Employee employee)
    {
        var authorizedPosition = employee.Position >= PositionType.Manager;

        return await Task.FromResult(authorizedPosition);
    }

    public async Task<bool> CanManageItemAsync(Employee employee, int itemId)
    {
        var item = await _dbContext.Items.FindAsync(itemId);
        var authorizedTenant = item != null ? item.TenantId == employee.TenantId : true;
        var authorizedPosition = employee.Position >= PositionType.Manager;

        return await Task.FromResult(authorizedTenant && authorizedPosition);
    }

    private async Task<bool> ItemExistsAsync(int itemId)
    {
        return await _dbContext.Items.AnyAsync(x => x.Id == itemId);
    }
}