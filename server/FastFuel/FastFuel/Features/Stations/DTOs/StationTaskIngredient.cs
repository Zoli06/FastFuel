namespace FastFuel.Features.Stations.DTOs;

public record StationTaskIngredient
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The quantity of the ingredient.
    /// </summary>
    public required uint Quantity { get; init; }
    /// <summary>
    /// The unit of measurement for the quantity.
    /// </summary>
    public required string Unit { get; init; }
    /// <summary>
    /// Indicates whether the ingredient is relevant for this task.
    /// </summary>
    public required bool IsRelevant { get; init; }
}