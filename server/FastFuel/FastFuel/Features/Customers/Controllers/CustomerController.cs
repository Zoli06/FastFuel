using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.Users.Controllers;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Customers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(
    IUserService<CustomerRequestDto, CustomerResponseDto> service)
    : CrudController<Customer, CustomerRequestDto, CustomerResponseDto>(service)
{
    /// <summary>
    /// Gets the profile of the currently authenticated customer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current customer profile.</returns>
    [HttpGet("me")]
    public Task<Results<Ok<CustomerResponseDto>, NotFound, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(service, User, cancellationToken);
    }

    /// <summary>
    /// Creates a new customer account.
    /// </summary>
    /// <param name="requestDto">The customer data to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created customer, or an error response if creation fails.</returns>
    [AllowAnonymous]
    public override
        Task<Results<Created<CustomerResponseDto>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>,
            UnauthorizedHttpResult, ForbidHttpResult>> Create(CustomerRequestDto requestDto,
            CancellationToken cancellationToken = default)
    {
        return base.Create(requestDto, cancellationToken);
    }

    /// <summary>
    /// Update the currently logged-in customer.
    /// </summary>
    /// <param name="requestDto">The updated customer data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content when the update succeeds; otherwise an error response.</returns>
    [HttpPut("me")]
    [PermissionCheck("UpdateSelf")]
    public async Task<Results<
        NoContent,
        NotFound,
        BadRequest<ProblemDetails>,
        Conflict<ProblemDetails>,
        UnauthorizedHttpResult,
        ForbidHttpResult>> UpdateSelf(CustomerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        return await base.Update(GetUserId(User)!.Value, requestDto, cancellationToken);
    }
}