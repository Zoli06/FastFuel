namespace FastFuel.Features.OrderMenus.DTOs;

public record OrderMenuResponseDto
{
    /// <summary>
    ///     The menu identifier.
    /// </summary>
    public required uint MenuId { get; init; }

    /// <summary>
    ///     The original menu name at the time of order placement.
    /// </summary>
    public required string OriginalMenuName { get; init; }

    /// <summary>
    ///     The original menu price at the time of order placement.
    /// </summary>
    public required double OriginalMenuPrice { get; init; }

    /// <summary>
    ///     The quantity of the menu in the order.
    /// </summary>
    public required uint Quantity { get; init; }

    /// <summary>
    ///     Additional instructions for preparation.
    /// </summary>
    public required string? SpecialInstructions { get; init; }
}