using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastFuel.Features.Authentication.DTOs;
using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FastFuel.Features.Authentication.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(
    ApplicationDbContext context,
    JwtSettings jwtSettings)
    : ControllerBase
{
    private readonly PasswordHasher<Restaurant> _hasher = new();

    [HttpPost("login")]
    public ActionResult<AuthenticationResponseDto> Login([FromBody] AuthenticationRequestDto dto)
    {
        var restaurant = context.Restaurants.FirstOrDefault(r => r.Id == dto.Id);
        if (restaurant == null)
            return NotFound(new { Message = "Restaurant not found" });

        var result = _hasher.VerifyHashedPassword(restaurant, restaurant.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return Unauthorized(new { Message = "Invalid password" });

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

        return Ok(new AuthenticationResponseDto
        {
            Message = "Login successful",
            Token = tokenString
        });
    }
}