namespace FastFuel.Features.Authentication.RestaurantAuthentication.DTOs;

public class RestaurantAuthenticationResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
}