namespace FastFuel.Features.Foods.DTOs;

public class EditFoodDto
{
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
}