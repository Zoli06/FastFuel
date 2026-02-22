using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Models;
using FastFuel.Features.Users.Controllers;
using FastFuel.Features.Users.Models;
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
    public IUserService<CustomerRequestDto, CustomerResponseDto> Service { get; } = service;
    public UserManager<User> UserManager { get; } = userManager;

    [HttpGet("me")]
    public async Task<Results<Ok<CustomerResponseDto>, UnauthorizedHttpResult>> GetCurrentUser()
    {
        return await ((IUserController<CustomerRequestDto, CustomerResponseDto>)this).GetCurrentUserDefault();
    }
}