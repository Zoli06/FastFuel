using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Employees.Services;

public class EmployeeService(
    ApplicationDbContext dbContext,
    IMapper<Employee, EmployeeRequestDto, EmployeeResponseDto> mapper,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
    : UserService<Employee, EmployeeRequestDto, EmployeeResponseDto>(dbContext, mapper, userManager, roleManager)
{
    protected override DbSet<Employee> DbSet { get; } = dbContext.Employees;

    protected override List<(string RoleName, string[] Permissions)> DefaultRoles =>
    [
        ..base.DefaultRoles,
        ("Employee",
        [
            "Permission:Shift:Read",
            "Permission:StationCategory:Read",
            "Permission:Employee:Read",
            "Permission:Order:Create",
            "Permission:Order:Read",
            "Permission:Order:Update",
            "Permission:Order:Delete",
            "Permission:Station:Read",
            "Permission:StationCategory:Read",
            "Permission:Customer:Read",
            "Permission:Station:ViewTasks"
        ])
    ];
}