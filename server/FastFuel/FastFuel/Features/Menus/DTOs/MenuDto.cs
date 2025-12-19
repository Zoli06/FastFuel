namespace FastFuel.Features.Menus.DTOs;

public class MenuDto
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public required List<MenuFoodDto> Foods { get; set; }
}