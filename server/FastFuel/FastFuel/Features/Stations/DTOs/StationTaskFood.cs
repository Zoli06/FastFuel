namespace FastFuel.Features.Stations.DTOs;

public record StationTaskFood
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
    /// The quantity of the food.
    /// </summary>
    public required uint Quantity { get; init; }
    /// <summary>
    /// Additional instructions for preparation.
    /// </summary>
    public required string? SpecialInstructions { get; init; }
    /// <summary>
    /// The list of ingredients.
    /// </summary>
    public required List<StationTaskIngredient> Ingredients { get; init; }
}