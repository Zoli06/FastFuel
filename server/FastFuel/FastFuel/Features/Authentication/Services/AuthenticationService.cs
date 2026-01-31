using FastFuel.Features.Authentication.DTOs;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Authentication.Services;

public class AuthenticationService(ApplicationDbContext dbContext, JwtSettings jwtSettings)
{
    public async Task<AuthenticationResponseDto?> AuthenticateAsync(AuthenticationRequestDto dto)
    {
        var restaurant = await dbContext.Restaurants
            .FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (restaurant == null)
            return null;
        var hasher = new PasswordHasher<Restaurant>();
        var result = hasher.VerifyHashedPassword(restaurant, restaurant.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return null;
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, restaurant.Id.ToString())
        };
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        return new AuthenticationResponseDto
        {
            Message = "Login successful",
            Token = tokenString
        };
    }
}