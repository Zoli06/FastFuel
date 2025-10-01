namespace FastFuel.Models;

public class SpecialRequest
{
    public uint Id { get; set; }
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;
    public required string Note { get; set; }
}