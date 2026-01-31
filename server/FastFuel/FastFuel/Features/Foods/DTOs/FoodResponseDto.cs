using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Foods.DTOs;

public class FoodResponseDto : IIdentifiable
{
    public string Name { get; init; } = string.Empty;
    public uint Price { get; init; }
    public string? Description { get; init; }
    public Uri? ImageUrl { get; init; }
    public List<FoodIngredientDto> Ingredients { get; init; } = [];
    public List<uint> MenuIds { get; init; } = [];
    public uint Id { get; init; }
}