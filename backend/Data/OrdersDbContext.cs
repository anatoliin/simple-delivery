using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasIndex(order => order.OrderNumber)
            .IsUnique();
    }
}
