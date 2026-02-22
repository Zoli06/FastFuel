namespace FastFuel.Features.Authentication.CustomerAuthentication.DTOs;

public class CustomerAuthenticationRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}