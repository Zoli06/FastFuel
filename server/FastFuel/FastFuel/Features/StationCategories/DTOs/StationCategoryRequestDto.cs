namespace FastFuel.Features.StationCategories.DTOs;

public record StationCategoryRequestDto
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The list of ingredient identifiers.
    /// </summary>
    public required List<uint> IngredientIds { get; init; }
}