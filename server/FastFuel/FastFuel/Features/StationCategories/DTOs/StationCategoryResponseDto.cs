using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.StationCategories.DTOs;

public record StationCategoryResponseDto : IIdentifiable
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The list of ingredient identifiers.
    /// </summary>
    public required List<uint> IngredientIds { get; init; }
    /// <summary>
    /// The list of station identifiers.
    /// </summary>
    public required List<uint> StationIds { get; init; }
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}