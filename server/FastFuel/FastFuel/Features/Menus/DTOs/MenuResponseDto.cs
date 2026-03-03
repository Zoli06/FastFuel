using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Menus.DTOs;

public record MenuResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required uint Price { get; init; }
    public required string? Description { get; init; }
    public required Uri? ImageUrl { get; init; }
    public required List<MenuFoodDto> Foods { get; init; }
    public required uint Id { get; init; }
}