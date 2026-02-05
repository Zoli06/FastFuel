using FastFuel.Features.Authentication.DTOs;

namespace FastFuel.Features.Authentication.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponseDto?> AuthenticateAsync(AuthenticationRequestDto dto);
}