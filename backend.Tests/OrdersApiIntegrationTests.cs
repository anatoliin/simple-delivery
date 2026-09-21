using System.Net;
using System.Net.Http.Json;
using backend.Contracts;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace backend.Tests;

public sealed class OrdersApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrdersApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task GetOrders_ReturnsHttpOkWithOrdersList()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        db.Orders.Add(new Order
        {
            OrderNumber = "DLV-000001",
            SenderCity = "Москва",
            SenderAddress = "ул. Ленина, 1",
            RecipientCity = "Казань",
            RecipientAddress = "ул. Пушкина, 2",
            Weight = 4m,
            PickupDate = new DateOnly(2026, 9, 21),
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
        Assert.NotNull(orders);
        Assert.NotEmpty(orders);
    }

    [Fact]
    public async Task PostOrder_WhenPayloadIsValid_ReturnsCreatedStatus()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        var request = new CreateOrderRequest(
            SenderCity: "Москва",
            SenderAddress: "ул. Ленина, 11",
            RecipientCity: "Казань",
            RecipientAddress: "ул. Гагарина, 7",
            Weight: 6.5m,
            PickupDate: new DateOnly(2026, 9, 23));

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Order>();
        Assert.NotNull(created);
        Assert.Equal("DLV-000001", created.OrderNumber);
    }

    [Theory]
    [InlineData("", "ул. Ленина, 11", "Казань", "ул. Гагарина, 7", 6.5, "2026-09-23")]
    [InlineData("Москва", "", "Казань", "ул. Гагарина, 7", 6.5, "2026-09-23")]
    [InlineData("Москва", "ул. Ленина, 11", "", "ул. Гагарина, 7", 6.5, "2026-09-23")]
    [InlineData("Москва", "ул. Ленина, 11", "Казань", "", 6.5, "2026-09-23")]
    [InlineData("Москва", "ул. Ленина, 11", "Казань", "ул. Гагарина, 7", 0, "2026-09-23")]
    [InlineData("Москва", "ул. Ленина, 11", "Казань", "ул. Гагарина, 7", 6.5, "")]
    public async Task PostOrder_WhenAnyRequiredParameterIsMissing_ReturnsBadRequest(
        string senderCity,
        string senderAddress,
        string recipientCity,
        string recipientAddress,
        decimal weight,
        string pickupDate)
    {
        var request = new CreateOrderRequest(
            SenderCity: senderCity,
            SenderAddress: senderAddress,
            RecipientCity: recipientCity,
            RecipientAddress: recipientAddress,
            Weight: weight,
            PickupDate: string.IsNullOrWhiteSpace(pickupDate) ? default : DateOnly.Parse(pickupDate));

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<OrdersDbContext>));

            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            services.AddSingleton(connection);
            services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlite(connection));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
