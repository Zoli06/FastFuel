namespace FastFuel.Features.Authentication.CustomerAuthentication.DTOs;

public class CustomerAuthenticationResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
}