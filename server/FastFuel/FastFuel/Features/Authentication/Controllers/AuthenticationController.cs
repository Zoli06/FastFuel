using FastFuel.Features.Authentication.DTOs;
using FastFuel.Features.Authentication.Services;
using FastFuel.Features.Common.DbContexts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Authentication.Controllers;

[ApiController]
[Route("api/Auth")]
public class AuthenticationController(
    ApplicationDbContext dbContext,
    JwtSettings jwtSettings)
    : ControllerBase
{
    private readonly AuthenticationService _authService = new(dbContext, jwtSettings);
    
    [HttpPost("login")]
    public async Task<Results<Ok<AuthenticationResponseDto>, UnauthorizedHttpResult>> Login(AuthenticationRequestDto dto)
    {
        var response = await _authService.AuthenticateAsync(dto);
        if (response == null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(response);
    }
}