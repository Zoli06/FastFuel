namespace FastFuel.Features.Restaurants.DTOs;

public record RestaurantRequestDto
{
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required string Address { get; init; }
    public required string? Phone { get; init; }
    public required List<RestaurantOpeningHourDto> OpeningHours { get; init; }
}