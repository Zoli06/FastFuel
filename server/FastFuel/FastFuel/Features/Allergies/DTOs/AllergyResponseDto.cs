using FastFuel.Features.Common;

namespace FastFuel.Features.Allergies.DTOs;

public class AllergyResponseDto : IIdentifiable
{
    public required string Name { get; set; }
    public string? Message { get; set; }
    public List<uint> IngredientIds { get; set; } = [];
    public uint Id { get; set; }
}