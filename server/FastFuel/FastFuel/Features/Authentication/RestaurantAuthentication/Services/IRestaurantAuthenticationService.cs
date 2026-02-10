using FastFuel.Features.Authentication.RestaurantAuthentication.DTOs;

namespace FastFuel.Features.Authentication.RestaurantAuthentication.Services;

public interface IRestaurantAuthenticationService
{
    Task<RestaurantAuthenticationResponseDto?> AuthenticateAsync(RestaurantAuthenticationRequestDto dto);
}