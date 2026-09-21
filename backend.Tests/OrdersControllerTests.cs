using backend.Contracts;
using backend.Controllers;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public sealed class OrdersControllerTests
{
    [Fact]
    public async Task GetOrders_ReturnsOrdersSortedByCreatedAtDescending()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var db = new OrdersDbContext(options))
        {
            await db.Database.EnsureCreatedAsync();
            db.Orders.AddRange(
                new Order
                {
                    OrderNumber = "DLV-000002",
                    SenderCity = "Казань",
                    SenderAddress = "ул. Ленина, 2",
                    RecipientCity = "Москва",
                    RecipientAddress = "ул. Пушкина, 3",
                    Weight = 5m,
                    PickupDate = new DateOnly(2026, 9, 20),
                    CreatedAt = new DateTime(2026, 9, 20, 8, 0, 0, DateTimeKind.Utc)
                },
                new Order
                {
                    OrderNumber = "DLV-000001",
                    SenderCity = "Москва",
                    SenderAddress = "ул. Пушкина, 3",
                    RecipientCity = "Казань",
                    RecipientAddress = "ул. Ленина, 2",
                    Weight = 3m,
                    PickupDate = new DateOnly(2026, 9, 19),
                    CreatedAt = new DateTime(2026, 9, 19, 9, 0, 0, DateTimeKind.Utc)
                });

            await db.SaveChangesAsync();
        }

        await using var dbContext = new OrdersDbContext(options);
        var controller = new OrdersController(dbContext);

        var result = await controller.GetOrders();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var orders = Assert.IsAssignableFrom<IReadOnlyList<Order>>(okResult.Value);

        Assert.Equal(2, orders.Count);
        Assert.Equal("DLV-000002", orders[0].OrderNumber);
        Assert.Equal("DLV-000001", orders[1].OrderNumber);
    }

    [Fact]
    public async Task GetOrder_WhenOrderExists_ReturnsOkResult()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var db = new OrdersDbContext(options))
        {
            await db.Database.EnsureCreatedAsync();
            db.Orders.Add(new Order
            {
                OrderNumber = "DLV-000001",
                SenderCity = "Москва",
                SenderAddress = "ул. Пушкина, 3",
                RecipientCity = "Казань",
                RecipientAddress = "ул. Ленина, 2",
                Weight = 2m,
                PickupDate = new DateOnly(2026, 9, 20),
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }

        await using var dbContext = new OrdersDbContext(options);
        var controller = new OrdersController(dbContext);

        var result = await controller.GetOrder(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var order = Assert.IsType<Order>(okResult.Value);
        Assert.Equal(1, order.Id);
        Assert.Equal("DLV-000001", order.OrderNumber);
    }

    [Fact]
    public async Task GetOrder_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new OrdersDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var controller = new OrdersController(db);

        var result = await controller.GetOrder(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateOrder_WhenRequestIsValid_ReturnsCreatedOrder()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new OrdersDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var controller = new OrdersController(db);
        var request = new CreateOrderRequest(
            SenderCity: "Москва",
            SenderAddress: "ул. Ленина, 10",
            RecipientCity: "Казань",
            RecipientAddress: "ул. Гагарина, 15",
            Weight: 12.5m,
            PickupDate: new DateOnly(2026, 9, 22));

        var result = await controller.CreateOrder(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(OrdersController.GetOrder), createdResult.ActionName);

        var order = Assert.IsType<Order>(createdResult.Value);
        Assert.Equal(1, order.Id);
        Assert.Equal("DLV-000001", order.OrderNumber);
        Assert.Equal("Москва", order.SenderCity);
        Assert.Equal(12.5m, order.Weight);
    }

    [Theory]
    [InlineData("", "ул. Ленина, 10", "Казань", "ул. Гагарина, 15", 12.5, "2026-09-22")]
    [InlineData("Москва", "", "Казань", "ул. Гагарина, 15", 12.5, "2026-09-22")]
    [InlineData("Москва", "ул. Ленина, 10", "", "ул. Гагарина, 15", 12.5, "2026-09-22")]
    [InlineData("Москва", "ул. Ленина, 10", "Казань", "", 12.5, "2026-09-22")]
    [InlineData("Москва", "ул. Ленина, 10", "Казань", "ул. Гагарина, 15", 0, "2026-09-22")]
    [InlineData("Москва", "ул. Ленина, 10", "Казань", "ул. Гагарина, 15", 12.5, "")]
    public async Task CreateOrder_WhenAnyRequiredParameterIsMissing_ReturnsBadRequest(
        string senderCity,
        string senderAddress,
        string recipientCity,
        string recipientAddress,
        decimal weight,
        string pickupDate)
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new OrdersDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var controller = new OrdersController(db);
        var request = new CreateOrderRequest(
            SenderCity: senderCity,
            SenderAddress: senderAddress,
            RecipientCity: recipientCity,
            RecipientAddress: recipientAddress,
            Weight: weight,
            PickupDate: string.IsNullOrWhiteSpace(pickupDate) ? default : DateOnly.Parse(pickupDate));

        var result = await controller.CreateOrder(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequest.Value);
    }
}
