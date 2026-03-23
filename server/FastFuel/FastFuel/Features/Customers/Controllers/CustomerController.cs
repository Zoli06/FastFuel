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
    public IUserService<CustomerRequestDto, CustomerResponseDto> UserService { get; } = service;

    [HttpGet("me")]
    public Task<Results<Ok<CustomerResponseDto>, NotFound, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(UserService, User, cancellationToken);
    }

    [AllowAnonymous]
    public override
        Task<Results<Created<CustomerResponseDto>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>,
            UnauthorizedHttpResult, ForbidHttpResult>> Create(CustomerRequestDto requestDto,
            CancellationToken cancellationToken = default)
    {
        return base.Create(requestDto, cancellationToken);
    }

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