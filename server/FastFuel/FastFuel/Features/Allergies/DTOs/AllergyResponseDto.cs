using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Allergies.DTOs;

public record AllergyResponseDto : IIdentifiable
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; } = string.Empty;
    /// <summary>
    /// The list of ingredient identifiers.
    /// </summary>
    public required List<uint> IngredientIds { get; init; } = [];
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}