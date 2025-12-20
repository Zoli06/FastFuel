namespace FastFuel.Features.Allergies.DTOs;

public class AllergyRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Message { get; set; }
    public List<uint> IngredientIds { get; set; } = [];
}