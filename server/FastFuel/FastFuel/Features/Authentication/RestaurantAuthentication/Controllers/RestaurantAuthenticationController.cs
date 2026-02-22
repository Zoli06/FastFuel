using FastFuel.Features.Authentication.RestaurantAuthentication.DTOs;
using FastFuel.Features.Authentication.RestaurantAuthentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Authentication.RestaurantAuthentication.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RestaurantAuthenticationController(
    IRestaurantAuthenticationService authService)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<Results<Ok<RestaurantAuthenticationResponseDto>, UnauthorizedHttpResult>> Login(
        RestaurantAuthenticationRequestDto dto)
    {
        var response = await authService.AuthenticateAsync(dto);
        if (response == null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(response);
    }
}