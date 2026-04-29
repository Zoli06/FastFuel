namespace FastFuel.Features.Stations.DTOs;

public record StationTaskMenu
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint? MenuId { get; init; }
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The quantity of the menu.
    /// </summary>
    public required uint Quantity { get; init; }
    /// <summary>
    /// Additional instructions for preparation.
    /// </summary>
    public required string? SpecialInstructions { get; init; }
    /// <summary>
    /// The list of foods.
    /// </summary>
    public required List<StationTaskFood> Foods { get; init; }
}