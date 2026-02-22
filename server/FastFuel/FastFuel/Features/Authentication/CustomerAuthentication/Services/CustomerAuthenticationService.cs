using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastFuel.Features.Authentication.CustomerAuthentication.DTOs;
using FastFuel.Features.Authentication.Settings;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Customers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FastFuel.Features.Authentication.CustomerAuthentication.Services;

public class CustomerAuthenticationService(
    ApplicationDbContext dbContext,
    IOptions<JwtSettings> jwtOptions,
    IPasswordHasher<Customer> passwordHasher) : ICustomerAuthenticationService
{
    public async Task<CustomerAuthenticationResponseDto?> AuthenticateAsync(CustomerAuthenticationRequestDto dto)
    {
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Username == dto.Username);
        if (customer == null)
            return null;
        var result = passwordHasher.VerifyHashedPassword(customer, customer.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return null;
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString())
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
        return new CustomerAuthenticationResponseDto
        {
            Message = "Login successful",
            Token = tokenString
        };
    }
}