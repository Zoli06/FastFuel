namespace FastFuel.Features.Authentication.DTOs;

public class AuthenticationRequestDto
{
    public uint Id { get; set; }
    public string Password { get; set; } = string.Empty;
}