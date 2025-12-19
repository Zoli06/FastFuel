namespace FastFuel.Features.Stations.DTOs;

public class StationDto
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public bool InOperation { get; set; }
    public uint RestaurantId { get; set; }
    public uint StationCategoryId { get; set; }
}