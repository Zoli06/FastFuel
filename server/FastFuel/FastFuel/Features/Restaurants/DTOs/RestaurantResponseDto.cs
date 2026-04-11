using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Restaurants.DTOs;

public record RestaurantResponseDto : IIdentifiable
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The description.
    /// </summary>
    public required string? Description { get; init; }
    /// <summary>
    /// The latitude.
    /// </summary>
    public required double Latitude { get; init; }
    /// <summary>
    /// The longitude.
    /// </summary>
    public required double Longitude { get; init; }
    /// <summary>
    /// The address.
    /// </summary>
    public required string Address { get; init; }
    /// <summary>
    /// The phone.
    /// </summary>
    public required string? Phone { get; init; }
    /// <summary>
    /// The list of opening hours.
    /// </summary>
    public required List<RestaurantOpeningHourDto> OpeningHours { get; init; }
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}