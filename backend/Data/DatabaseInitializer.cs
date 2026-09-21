using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await db.Database.EnsureCreatedAsync();

        var orders = await db.Orders.ToListAsync();
        foreach (var order in orders)
        {
            var expectedNumber = $"DLV-{order.Id:D6}";
            if (order.OrderNumber != expectedNumber)
            {
                order.OrderNumber = expectedNumber;
            }
        }

        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync(
            "CREATE UNIQUE INDEX IF NOT EXISTS IX_Orders_OrderNumber ON Orders (OrderNumber)");
    }
}
