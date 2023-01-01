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

    public async Task<bool> CanManageOrdersAsync(Employee employee)
    {
        var authorized = employee.Position == PositionType.Cashier || employee.Position == PositionType.Manager || employee.Position == PositionType.Admin;
        return await Task.FromResult(authorized);
    }
}