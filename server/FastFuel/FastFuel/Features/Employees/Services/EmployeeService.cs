using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Employees.Services;

public class EmployeeService(
    ApplicationDbContext dbContext,
    IMapper<Employee, EmployeeRequestDto, EmployeeResponseDto> mapper,
    IPasswordHasher<Employee> passwordHasher)
    : CrudService<Employee, EmployeeRequestDto, EmployeeResponseDto>(dbContext, mapper)
{
    protected override DbSet<Employee> DbSet { get; } = dbContext.Employees;

    protected override Task OnBeforeCreateModelAsync(Employee model, EmployeeRequestDto requestDto)
    {
        if (requestDto.Password == null)
            throw new ArgumentException("Password is required for employee creation.");
        model.PasswordHash = passwordHasher.HashPassword(model, requestDto.Password);
        return Task.CompletedTask;
    }

    protected override Task OnBeforeUpdateModelAsync(Employee model, EmployeeRequestDto requestDto)
    {
        if (requestDto.Password != null) model.PasswordHash = passwordHasher.HashPassword(model, requestDto.Password);
        return Task.CompletedTask;
    }
}