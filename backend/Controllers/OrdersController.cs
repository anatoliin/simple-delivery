using backend.Contracts;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(OrdersDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrders()
    {
        var orders = await db.Orders
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await db.Orders.FindAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequest request)
    {
        if (!IsValid(request))
        {
            return BadRequest(new { message = "Заполните все поля корректно." });
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        var order = new Order
        {
            OrderNumber = $"PENDING-{Guid.NewGuid():N}",
            SenderCity = request.SenderCity!.Trim(),
            SenderAddress = request.SenderAddress!.Trim(),
            RecipientCity = request.RecipientCity!.Trim(),
            RecipientAddress = request.RecipientAddress!.Trim(),
            Weight = request.Weight,
            PickupDate = request.PickupDate,
            CreatedAt = DateTime.UtcNow
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        order.OrderNumber = $"DLV-{order.Id:D6}";
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    private static bool IsValid(CreateOrderRequest request) =>
        !string.IsNullOrWhiteSpace(request.SenderCity) &&
        !string.IsNullOrWhiteSpace(request.SenderAddress) &&
        !string.IsNullOrWhiteSpace(request.RecipientCity) &&
        !string.IsNullOrWhiteSpace(request.RecipientAddress) &&
        request.Weight > 0 &&
        request.PickupDate != default;
}
