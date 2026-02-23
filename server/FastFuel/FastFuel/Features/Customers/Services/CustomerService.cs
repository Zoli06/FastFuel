using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Models;
using FastFuel.Features.Roles.Models;
using FastFuel.Features.Users.Models;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Customers.Services;

public class CustomerService(
    ApplicationDbContext dbContext,
    IMapper<Customer, CustomerRequestDto, CustomerResponseDto> mapper,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
    : UserService<Customer, CustomerRequestDto, CustomerResponseDto>(dbContext, mapper, userManager, roleManager)
{
    protected override DbSet<Customer> DbSet { get; } = dbContext.Customers;

    protected override List<(string RoleName, string[] Permissions)> DefaultRoles =>
    [
        ..base.DefaultRoles,
        ("Customer", ["Permission:Order:Create"])
    ];
}