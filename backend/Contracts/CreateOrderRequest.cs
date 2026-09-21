namespace backend.Contracts;

public sealed record CreateOrderRequest(
    string? SenderCity,
    string? SenderAddress,
    string? RecipientCity,
    string? RecipientAddress,
    decimal Weight,
    DateOnly PickupDate);
