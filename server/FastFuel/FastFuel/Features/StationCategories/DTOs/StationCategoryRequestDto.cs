namespace FastFuel.Features.StationCategories.DTOs;

public class StationCategoryRequestDto
{
    public string Name { get; init; } = string.Empty;
    public List<uint> IngredientIds { get; init; } = [];
    public List<uint> StationIds { get; init; } = [];
}