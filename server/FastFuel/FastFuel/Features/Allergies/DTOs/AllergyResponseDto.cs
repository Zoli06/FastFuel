using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Allergies.DTOs;

public record AllergyResponseDto : IIdentifiable
{
    public required string Name { get; init; } = string.Empty;
    public required string? Message { get; init; }
    public required List<uint> IngredientIds { get; init; } = [];
    public required uint Id { get; init; }
}