namespace FastFuel.Features.Allergies.DTOs;

public record AllergyRequestDto
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The message.
    /// </summary>
    public required string? Message { get; init; }
    /// <summary>
    /// The list of ingredient identifiers.
    /// </summary>
    public required List<uint> IngredientIds { get; init; } = [];
}