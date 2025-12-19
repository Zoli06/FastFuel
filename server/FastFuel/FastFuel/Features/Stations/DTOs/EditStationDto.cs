namespace FastFuel.Features.Stations.DTOs;

public class EditStationDto
{
    // TODO: Do something about strings, they aren't supposed to be required
    public required string Name { get; set; }
    public bool InOperation { get; set; }
    public uint RestaurantId { get; set; }
    public uint StationCategoryId { get; set; }
}