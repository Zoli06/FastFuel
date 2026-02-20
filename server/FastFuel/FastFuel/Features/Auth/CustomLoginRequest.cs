// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace FastFuel.Features.Auth;

/// <summary>
///     The request type for the "/login" endpoint added by
///     <see cref="IdentityApiEndpointRouteBuilderExtensions.MapIdentityApi" />.
/// </summary>
public sealed class CustomLoginRequest
{
    /// <summary>
    ///     The user's email address which acts as a user name.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     The user's password.
    /// </summary>
    public required string Password { get; init; }
}