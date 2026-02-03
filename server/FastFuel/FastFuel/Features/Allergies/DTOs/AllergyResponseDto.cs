using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Allergies.DTOs;

public class AllergyResponseDto : IIdentifiable
{
    public string Name { get; init; } = string.Empty;
    public string? Message { get; init; }
    public List<uint> IngredientIds { get; init; } = [];
    public uint Id { get; init; }
}