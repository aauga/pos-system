using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Employee> Employees { get; }
    DbSet<Order> Orders { get; }
    DbSet<Cart> Carts { get; }
    DbSet<Delivery> Deliveries { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Item> Items { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync();
}