namespace FastFuel.Features.Authentication.RestaurantAuthentication.DTOs;

public class RestaurantAuthenticationRequestDto
{
    public uint Id { get; set; }
    public string Password { get; set; } = string.Empty;
}