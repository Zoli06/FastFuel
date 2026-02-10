using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Customers.Services;

public class CustomerService(
    ApplicationDbContext dbContext,
    IMapper<Customer, CustomerRequestDto, CustomerResponseDto> mapper,
    IPasswordHasher<Customer> passwordHasher)
    : CrudService<Customer, CustomerRequestDto, CustomerResponseDto>(dbContext, mapper)
{
    protected override DbSet<Customer> DbSet { get; } = dbContext.Customers;

    protected override Task OnBeforeCreateModelAsync(Customer model)
    {
        model.PasswordHash = passwordHasher.HashPassword(model, model.PasswordHash);
        return Task.CompletedTask;
    }
}