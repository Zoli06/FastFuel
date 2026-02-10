using FastFuel.Features.Authentication.CustomerAuthentication.DTOs;

namespace FastFuel.Features.Authentication.CustomerAuthentication.Services;

public interface ICustomerAuthenticationService
{
    Task<CustomerAuthenticationResponseDto?> AuthenticateAsync(CustomerAuthenticationRequestDto dto);
}