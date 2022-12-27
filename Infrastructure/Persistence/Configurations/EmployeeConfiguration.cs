using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasOne(x => x.Tenant).WithMany(x => x.Employees);
        builder.HasMany(x => x.Orders).WithOne(x => x.Employee);
    }
}