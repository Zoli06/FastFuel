namespace FastFuel.Features.Authentication.DTOs;

public class AuthenticationResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
}