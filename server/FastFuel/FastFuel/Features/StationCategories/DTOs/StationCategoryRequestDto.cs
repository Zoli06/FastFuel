namespace FastFuel.Features.StationCategories.DTOs;

public class StationCategoryRequestDto
{
    public string Name { get; init; } = string.Empty;
    public virtual List<uint> IngredientIds { get; init; } = [];
    public virtual List<uint> StationIds { get; init; } = [];
}