namespace FastFuel.Features.Authentication.Settings;

public class JwtSettings(string secretKey, string issuer, string audience, int expirationMinutes) : IJwtSettings
{
    public string SecretKey { get; } = secretKey;
    public string Issuer { get; } = issuer;
    public string Audience { get; } = audience;
    public int ExpirationMinutes { get; } = expirationMinutes;
}