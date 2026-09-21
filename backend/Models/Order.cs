using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public sealed class Order
{
    [Key]
    public int Id { get; set; }
    public required string OrderNumber { get; set; }
    public required string SenderCity { get; set; }
    public required string SenderAddress { get; set; }
    public required string RecipientCity { get; set; }
    public required string RecipientAddress { get; set; }
    public decimal Weight { get; set; }
    public DateOnly PickupDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
