namespace FastFuel.Features.Orders.DTOs;

public record OrderMenuDto
{
    /// <summary>
    /// The menu identifier.
    /// </summary>
    public required uint MenuId { get; init; }
    /// <summary>
    /// The quantity of the menu in the order.
    /// </summary>
    public required uint Quantity { get; init; }
    /// <summary>
    /// Additional instructions for preparation.
    /// </summary>
    public required string? SpecialInstructions { get; init; }
}