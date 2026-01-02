using FastFuel.Features.Common;

namespace FastFuel.Features.Stations.DTOs;

public class StationResponseDto : IIdentifiable
{
    public uint Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool InOperation { get; init; }
    public uint RestaurantId { get; init; }
    public uint StationCategoryId { get; init; }
}