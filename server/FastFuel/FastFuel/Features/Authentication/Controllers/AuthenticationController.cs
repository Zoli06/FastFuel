using FastFuel.Features.Authentication.DTOs;
using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastFuel.Features.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(ApplicationDbContext context) : ControllerBase
{
    private readonly PasswordHasher<Restaurant> _hasher = new PasswordHasher<Restaurant>();
    private const string JwtSecretKey = "YourSuperSecretKeyHere:Mordekaiser"; // move appsettings.json 
    private const string JwtIssuer = "FastFuelAPI";
    private const string JwtAudience = "FastFuelClients";

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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), 
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new AuthenticationResponseDto
        {
            Message = "Login successful",
            Token = tokenString
        });
    }
}
