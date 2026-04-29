namespace FastFuel.Features.Menus.DTOs;

public record MenuRequestDto
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
    /// The list of foods.
    /// </summary>
    public required List<MenuFoodDto> Foods { get; init; }
}