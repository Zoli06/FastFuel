namespace FastFuel.Features.Orders.DTOs;

public class OrderMenuDto
{
    public uint MenuId { get; init; }
    public uint Quantity { get; init; }
    public string? SpecialInstructions { get; init; }
}