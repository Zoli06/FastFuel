namespace FastFuel.Features.Allergies.DTOs;

public record AllergyRequestDto
{
    public required string Name { get; init; }
    public required string? Message { get; init; }
    public required List<uint> IngredientIds { get; init; } = [];
}