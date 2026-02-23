namespace FastFuel.Features.Orders.DTOs;

public record OrderRequestDto
{
    public required uint? CustomerId { get; init; } //TODO: check if the logined CustomerId is same as the one we got
    public required uint RestaurantId { get; init; }
    public required List<OrderMenuDto> Menus { get; init; }
    public required List<OrderFoodDto> Foods { get; init; }
}