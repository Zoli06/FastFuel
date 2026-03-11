using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Roles.Services;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Employees.Services;

public class EmployeeService(
    ApplicationDbContext dbContext,
    IMapper<Employee, EmployeeRequestDto, EmployeeResponseDto> mapper,
    UserManager<User> userManager)
    : UserServiceBase<Employee, EmployeeRequestDto, EmployeeResponseDto>(dbContext, mapper, userManager)
{
    protected override DbSet<Employee> DbSet { get; } = dbContext.Employees;

    protected override DefaultRole[] DefaultRoles =>
    [
        ..base.DefaultRoles,
        DefaultRole.Employee
    ];
}