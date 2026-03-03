using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Stations.DTOs;

public record StationResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required bool InOperation { get; init; }
    public required uint RestaurantId { get; init; }
    public required uint StationCategoryId { get; init; }
    public required uint Id { get; init; }
}