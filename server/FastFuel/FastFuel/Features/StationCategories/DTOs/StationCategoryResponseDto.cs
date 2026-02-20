using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.StationCategories.DTOs;

public record StationCategoryResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required List<uint> IngredientIds { get; init; }
    public required List<uint> StationIds { get; init; }
    public required uint Id { get; init; }
}