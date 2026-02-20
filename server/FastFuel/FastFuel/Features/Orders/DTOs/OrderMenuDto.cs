namespace FastFuel.Features.Orders.DTOs;

public record OrderMenuDto
{
    public required uint MenuId { get; init; }
    public required uint Quantity { get; init; }
    public required string? SpecialInstructions { get; init; }
}