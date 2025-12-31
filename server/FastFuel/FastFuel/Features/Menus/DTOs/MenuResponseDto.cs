using FastFuel.Features.Common;

namespace FastFuel.Features.Menus.DTOs;

public class MenuResponseDto : IIdentifiable
{
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public required List<MenuFoodDto> Foods { get; set; }
    public uint Id { get; set; }
}