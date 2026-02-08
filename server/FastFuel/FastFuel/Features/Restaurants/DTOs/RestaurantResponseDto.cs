using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Restaurants.DTOs;

public class RestaurantResponseDto : IIdentifiable
{
    public uint Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Address { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public List<RestaurantOpeningHourDto> OpeningHours { get; init; } = [];
}