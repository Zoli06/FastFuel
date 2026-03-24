using FastFuel.Features.Auth.DTOs;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FastFuel.Features.Auth.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController(
    SignInManager<User> signInManager,
    TimeProvider timeProvider,
    IOptionsMonitor<BearerTokenOptions> bearerTokenOptions)
    : ControllerBase
{
    /// <summary>
    /// Authenticates a user with a username and password.
    /// </summary>
    /// <param name="login">The login credentials.</param>
    /// <param name="useCookies">Whether to use cookies for authentication.</param>
    /// <param name="useSessionCookies">Whether to use session cookies for authentication.</param>
    /// <returns>
    /// An access token response for bearer authentication, an empty result for cookie authentication,
    /// or a problem response when authentication fails.
    /// </returns>
    [HttpPost("login")]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login(
        LoginRequestDto login,
        bool? useCookies,
        bool? useSessionCookies)
    {
        var useCookieScheme = useCookies == true || useSessionCookies == true;
        var isPersistent = useCookies == true && useSessionCookies != true;
        signInManager.AuthenticationScheme =
            useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent, true);

        if (!result.Succeeded)
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);

        return TypedResults.Empty;
    }

    /// <summary>
    /// Refreshes an access token by using a refresh token.
    /// </summary>
    /// <param name="refreshRequest">The refresh token payload.</param>
    /// <returns>A new bearer sign-in result when the refresh token is valid; otherwise a challenge response.</returns>
    [HttpPost("refresh")]
    public async Task<Results<UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh(
        RefreshRequest refreshRequest)
    {
        var refreshTokenProtector =
            bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        if (refreshTicket?.Properties.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)
            return TypedResults.Challenge();

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }

    /// <summary>
    /// Signs the current user out.
    /// </summary>
    /// <returns>An OK result when sign-out completes.</returns>
    [HttpPost("logout")]
    public async Task<Results<Ok, UnauthorizedHttpResult>> Logout()
    {
        await signInManager.SignOutAsync();
        return TypedResults.Ok();
    }
}