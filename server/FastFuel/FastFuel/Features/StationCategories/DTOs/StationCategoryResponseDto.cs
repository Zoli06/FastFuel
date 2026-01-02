using FastFuel.Features.Common;

namespace FastFuel.Features.StationCategories.DTOs;

public class StationCategoryResponseDto : IIdentifiable
{
    public uint Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public virtual List<uint> IngredientIds { get; init; } = [];
    public virtual List<uint> StationIds { get; init; } = [];
}