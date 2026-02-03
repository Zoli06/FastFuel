namespace FastFuel.Features.Authentication.Settings;

public interface IJwtSettings
{
    string SecretKey { get; }
    string Issuer { get; }
    string Audience { get; }
    int ExpirationMinutes { get; }
}