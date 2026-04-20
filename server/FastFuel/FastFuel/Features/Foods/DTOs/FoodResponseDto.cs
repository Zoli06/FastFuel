using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Foods.DTOs;

public record FoodResponseDto : IIdentifiable
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The price.
    /// </summary>
    public required double Price { get; init; }
    /// <summary>
    /// The description.
    /// </summary>
    public required string? Description { get; init; }
    /// <summary>
    /// The image url.
    /// </summary>
    public required Uri? ImageUrl { get; init; }
    /// <summary>
    /// The list of ingredients.
    /// </summary>
    public required List<FoodIngredientDto> Ingredients { get; init; }
    /// <summary>
    /// The list of menu identifiers.
    /// </summary>
    public required List<uint> MenuIds { get; init; }
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}