namespace FastFuel.Features.StationCategories.DTOs;

public record StationCategoryRequestDto
{
    public required string Name { get; init; }
    public required List<uint> IngredientIds { get; init; }
    public required List<uint> StationIds { get; init; }
}