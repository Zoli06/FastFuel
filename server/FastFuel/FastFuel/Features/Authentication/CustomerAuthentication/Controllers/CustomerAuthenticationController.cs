using FastFuel.Features.Authentication.CustomerAuthentication.DTOs;
using FastFuel.Features.Authentication.CustomerAuthentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Authentication.CustomerAuthentication.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CustomerAuthenticationController(
    ICustomerAuthenticationService authService)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<Results<Ok<CustomerAuthenticationResponseDto>, UnauthorizedHttpResult>> Login(
        CustomerAuthenticationRequestDto dto)
    {
        var response = await authService.AuthenticateAsync(dto);
        if (response == null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(response);
    }
}