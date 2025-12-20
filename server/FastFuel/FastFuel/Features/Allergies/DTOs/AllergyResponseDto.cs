namespace FastFuel.Features.Allergies.DTOs;

public class AllergyResponseDto
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public string? Message { get; set; }
    public List<uint> IngredientIds { get; set; } = [];
}