namespace FastFuel.Features.Stations.DTOs;

public record StationRequestDto
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The restaurant identifier.
    /// </summary>
    public required uint RestaurantId { get; init; }
    /// <summary>
    /// The station category identifier.
    /// </summary>
    public required uint StationCategoryId { get; init; }
}