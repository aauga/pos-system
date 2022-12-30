using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_dbContext.Database.IsSqlite())
            {
                await _dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        var tenantsExist = _dbContext.Tenants.Any();

        if (!tenantsExist)
        {
            var tenant = SeedTenant();
            _dbContext.Tenants.Add(tenant);
        }

        var employeesExist = _dbContext.Employees.Any();

        if (!employeesExist)
        {
            var employee = SeedEmployee(tenantsExist);
            _dbContext.Employees.Add(employee);
        }

        await _dbContext.SaveChangesAsync();
    }

    private Tenant SeedTenant()
    {
        return new Tenant
        {
            Id = 1,
            Name = "ADMIN_TENANT",
            ActiveFrom = DateTime.MinValue,
            ActiveTo = DateTime.MaxValue
        };
    }

    private Employee SeedEmployee(bool tenantsExist)
    {
        return new Employee
        {
            Username = "admin",
            Password = "admin",
            Position = PositionType.Admin,
            TenantId = 1,
        };
    }
}
