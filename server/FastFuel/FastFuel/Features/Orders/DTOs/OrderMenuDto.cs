namespace FastFuel.Features.Orders.DTOs;

public class OrderMenuDto
{
    public uint MenuId { get; set; }
    public uint Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}