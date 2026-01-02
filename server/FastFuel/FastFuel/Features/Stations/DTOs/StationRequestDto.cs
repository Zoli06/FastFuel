namespace FastFuel.Features.Stations.DTOs;

public class StationRequestDto
{
    public string Name { get; init; } = string.Empty;
    public bool InOperation { get; init; }
    public uint RestaurantId { get; init; }
    public uint StationCategoryId { get; init; }
}