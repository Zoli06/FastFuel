namespace FastFuel.Models;

public class Station
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public bool InOperation { get; set; }
    public uint RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;
    public uint StationTypeId { get; set; }
    public StationType StationType { get; set; } = null!;
}