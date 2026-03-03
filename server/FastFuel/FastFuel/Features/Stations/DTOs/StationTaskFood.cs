namespace FastFuel.Features.Stations.DTOs;

public record StationTaskFood
{
    public required uint Id { get; init; }
    public required string Name { get; init; }
    public required uint Quantity { get; init; }
    public required string? SpecialInstructions { get; init; }
    public required List<StationTaskIngredient> Ingredients { get; init; }
}