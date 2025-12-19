namespace FastFuel.Features.Allergies.DTOs;

public class EditAllergyDto
{
    public required string Name { get; set; }
    public string? Message { get; set; }
    public virtual List<uint> IngredientIds { get; set; } = [];
}