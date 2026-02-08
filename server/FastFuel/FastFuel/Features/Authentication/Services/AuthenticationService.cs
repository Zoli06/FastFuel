using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastFuel.Features.Authentication.DTOs;
using FastFuel.Features.Authentication.Settings;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FastFuel.Features.Authentication.Services;

public class AuthenticationService(
    ApplicationDbContext dbContext,
    IOptions<JwtSettings> jwtOptions,
    IPasswordHasher<Restaurant> passwordHasher) : IAuthenticationService
{
    public async Task<AuthenticationResponseDto?> AuthenticateAsync(AuthenticationRequestDto dto)
    {
        var restaurant = await dbContext.Restaurants
            .FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (restaurant == null)
            return null;
        var result = passwordHasher.VerifyHashedPassword(restaurant, restaurant.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return null;
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, restaurant.Id.ToString())
        };
        var jwtSettings = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthenticationResponseDto
        {
            Message = "Login successful",
            Token = tokenString
        };
    }
}