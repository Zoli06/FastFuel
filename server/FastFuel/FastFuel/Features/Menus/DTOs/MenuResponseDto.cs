using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Menus.DTOs;

public class MenuResponseDto : IIdentifiable
{
    public string Name { get; init; } = string.Empty;
    public uint Price { get; init; }
    public string? Description { get; init; }
    public Uri? ImageUrl { get; init; }
    public required List<MenuFoodDto> Foods { get; init; }
    public uint Id { get; init; }
}