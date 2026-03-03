using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
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

    protected override Update<Customer, CustomerRequestDto, CustomerResponseDto> UpdateOperation { get; } =
        new Update(dbContext, dbContext.Customers, mapper);

    protected override List<(string RoleName, string[] Permissions)> DefaultRoles =>
    [
        ..base.DefaultRoles,
        ("Customer", ["Permission:Order:Create"])
    ];

    private class Update(
        ApplicationDbContext dbContext,
        DbSet<Customer> dbSet,
        IMapper<Customer, CustomerRequestDto, CustomerResponseDto> mapper)
        : Update<Customer, CustomerRequestDto, CustomerResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task<Customer?> GetEntityAsync(uint id, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            var customer = await base.GetEntityAsync(id, userId, cancellationToken)
                           ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");

            if (userId.HasValue && customer.Id != userId.Value)
                throw new UnauthorizedAccessException("You do not have permission to update this customer.");

            return customer;
        }
    }
}