using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.StationCategories.DTOs;

public class StationCategoryResponseDto : IIdentifiable
{
    public string Name { get; init; } = string.Empty;
    public List<uint> IngredientIds { get; init; } = [];
    public List<uint> StationIds { get; init; } = [];
    public uint Id { get; init; }
}