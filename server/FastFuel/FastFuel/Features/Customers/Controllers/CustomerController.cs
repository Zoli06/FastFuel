using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.Users.Controllers;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Customers.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomerController(
    IUserService<CustomerRequestDto, CustomerResponseDto> service,
    UserManager<User> userManager)
    : CrudController<Customer, CustomerRequestDto, CustomerResponseDto>(service),
        IUserController<CustomerRequestDto, CustomerResponseDto>
{
    public IUserService<CustomerRequestDto, CustomerResponseDto> UserService { get; } = service;
    public UserManager<User> UserManager { get; } = userManager;

    [HttpGet("me")]
    public async Task<Results<Ok<CustomerResponseDto>, NotFound, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return await ((IUserController<CustomerRequestDto, CustomerResponseDto>)this).GetCurrentUserDefault(
            cancellationToken);
    }

    [AllowAnonymous]
    public override
        Task<Results<Created<CustomerResponseDto>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>,
            UnauthorizedHttpResult, ForbidHttpResult>> Create(CustomerRequestDto requestDto,
            CancellationToken cancellationToken = default)
    {
        return base.Create(requestDto, cancellationToken);
    }

    // Custom permission check in the service to only allow customers to update their own data
    // Even admins can't update customers
    // They can delete it though
    [SkipPermissionCheck]
    public override Task<Results<
        NoContent,
        NotFound,
        BadRequest<ProblemDetails>,
        Conflict<ProblemDetails>,
        UnauthorizedHttpResult,
        ForbidHttpResult>> Update(uint id, CustomerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        return base.Update(id, requestDto, cancellationToken);
    }
}