namespace FastFuel.Features.Stations.DTOs;

public record StationTaskMenu
{
    public required uint Id { get; init; }
    public required string Name { get; init; }
    public required uint Quantity { get; init; }
    public required string? SpecialInstructions { get; init; }
    public required List<StationTaskFood> Foods { get; init; }
}