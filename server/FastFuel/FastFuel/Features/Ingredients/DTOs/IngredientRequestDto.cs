namespace FastFuel.Features.Ingredients.DTOs;

public record IngredientRequestDto
{
    /// <summary>
    ///     The displayed name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The list of allergy identifiers.
    /// </summary>
    public required List<uint> AllergyIds { get; init; }

    /// <summary>
    ///     The list of station category identifiers.
    /// </summary>
    public required List<uint> StationCategoryIds { get; init; }
}