using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastFuel.Features.Authentication.DTOs;
using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FastFuel.Features.Authentication.Controllers;

[ApiController]
[Route("api/Auth")]
public class AuthenticationController(
    ApplicationDbContext context,
    JwtSettings jwtSettings)
    : ControllerBase
{
    private readonly PasswordHasher<Restaurant> _hasher = new();

    [HttpPost("login")]
    public async Task<Results<Ok<AuthenticationResponseDto>, UnauthorizedHttpResult>> Login(AuthenticationRequestDto dto)
    {
        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (restaurant == null)
            return TypedResults.Unauthorized();

        var result = _hasher.VerifyHashedPassword(restaurant, restaurant.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return TypedResults.Unauthorized();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, restaurant.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return TypedResults.Ok(new AuthenticationResponseDto
        {
            Message = "Login successful",
            Token = tokenString
        });
    }
}