using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Models;
using FastFuel.Features.Users.Models;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Employees.Services;

public class EmployeeService(
    ApplicationDbContext dbContext,
    IMapper<Employee, EmployeeRequestDto, EmployeeResponseDto> mapper,
    IPasswordHasher<User> passwordHasher)
    : UserService<Employee, EmployeeRequestDto, EmployeeResponseDto>(dbContext, mapper, passwordHasher)
{
    protected override DbSet<Employee> DbSet { get; } = dbContext.Employees;
}