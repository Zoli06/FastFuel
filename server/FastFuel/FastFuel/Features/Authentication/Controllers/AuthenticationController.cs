using FastFuel.Features.Authentication.DTOs;
using FastFuel.Features.Authentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Authentication.Controllers;

[ApiController]
[Route("api/Auth")]
public class AuthenticationController(
    IAuthenticationService authService)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<Results<Ok<AuthenticationResponseDto>, UnauthorizedHttpResult>> Login(
        AuthenticationRequestDto dto)
    {
        var response = await authService.AuthenticateAsync(dto);
        if (response == null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(response);
    }
}