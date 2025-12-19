namespace FastFuel.Features.StationCategories.DTOs;

public class StationCategoryDto
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public virtual List<uint> IngredientIds { get; set; } = [];
    public virtual List<uint> StationIds { get; set; } = [];
}