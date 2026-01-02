namespace FastFuel.Features.Allergies.DTOs;

public class AllergyRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string? Message { get; init; }
    public List<uint> IngredientIds { get; init; } = [];
}