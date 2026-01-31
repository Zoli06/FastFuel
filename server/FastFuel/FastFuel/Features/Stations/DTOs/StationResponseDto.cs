using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Stations.DTOs;

public class StationResponseDto : IIdentifiable
{
    public string Name { get; init; } = string.Empty;
    public bool InOperation { get; init; }
    public uint RestaurantId { get; init; }
    public uint StationCategoryId { get; init; }
    public uint Id { get; init; }
}