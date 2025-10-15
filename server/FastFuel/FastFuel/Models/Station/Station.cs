namespace FastFuel.Models;

public class Station : BaseModel
{
    public required string Name { get; set; }
    public bool InOperation { get; set; }
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    public uint StationCategoryId { get; set; }
    public virtual StationCategory StationCategory { get; set; } = null!;
}