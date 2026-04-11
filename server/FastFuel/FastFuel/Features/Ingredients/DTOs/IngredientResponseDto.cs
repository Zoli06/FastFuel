using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Ingredients.DTOs;

public record IngredientResponseDto : IIdentifiable
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The image url.
    /// </summary>
    public required Uri? ImageUrl { get; init; }
    /// <summary>
    /// The list of food identifiers.
    /// </summary>
    public required List<uint> FoodIds { get; init; }
    /// <summary>
    /// The list of allergy identifiers.
    /// </summary>
    public required List<uint> AllergyIds { get; init; }
    /// <summary>
    /// The list of station category identifiers.
    /// </summary>
    public required List<uint> StationCategoryIds { get; init; }
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}