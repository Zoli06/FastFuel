namespace FastFuel.Features.Stations.DTOs;

public record StationTaskIngredient
{
    public required uint Id { get; init; }
    public required string Name { get; init; }
    public required uint Quantity { get; init; }
    public required string Unit { get; init; }
    public required bool IsRelevant { get; init; }
}