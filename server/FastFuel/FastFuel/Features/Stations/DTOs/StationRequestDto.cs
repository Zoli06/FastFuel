namespace FastFuel.Features.Stations.DTOs;

public record StationRequestDto
{
    public required string Name { get; init; }
    public required bool InOperation { get; init; }
    public required uint RestaurantId { get; init; }
    public required uint StationCategoryId { get; init; }
}