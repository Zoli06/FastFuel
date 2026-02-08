namespace FastFuel.Features.Restaurants.DTOs;

public class RestaurantRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Address { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public List<RestaurantOpeningHourDto> OpeningHours { get; init; } = [];
    public string Password { get; set; } = string.Empty;
}