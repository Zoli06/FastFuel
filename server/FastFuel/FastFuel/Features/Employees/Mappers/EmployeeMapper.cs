using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Models;
using FastFuel.Features.Roles.Models;
using FastFuel.Features.Users.Mappers;
using FastFuel.Features.Users.Models;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Employees.Mappers;

public class EmployeeMapper(
    ApplicationDbContext dbContext,
    RoleManager<Role> roleManager,
    UserManager<User> userManager)
    : UserMapper(roleManager, userManager), IMapper<Employee, EmployeeRequestDto, EmployeeResponseDto>
{
    protected override string UserType => "Employee";

    public EmployeeResponseDto ToDto(Employee model)
    {
        var userDto = base.ToDto(model);
        return new EmployeeResponseDto
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Email = userDto.Email,
            UserName = userDto.UserName,
            ThemeId = userDto.ThemeId,
            RoleIds = userDto.RoleIds,
            UserType = userDto.UserType,
            ShiftIds = model.Shifts.ConvertAll(shift => shift.Id),
            StationCategoryIds = model.StationCategories.ConvertAll(category => category.Id)
        };
    }

    public Employee ToModel(EmployeeRequestDto dto)
    {
        var userModel = base.ToModel(dto);
        return new Employee
        {
            Id = userModel.Id,
            Name = userModel.Name,
            Email = userModel.Email,
            UserName = userModel.UserName,
            ThemeId = userModel.ThemeId,
            Shifts = dbContext.Shifts.Where(s => dto.ShiftIds.Contains(s.Id)).ToList(),
            StationCategories = dbContext.StationCategories.Where(sc => dto.StationCategoryIds.Contains(sc.Id)).ToList()
        };
    }

    public void UpdateModel(EmployeeRequestDto dto, Employee model)
    {
        User userModel = model;
        base.UpdateModel(dto, userModel);
        model.Shifts = dbContext.Shifts.Where(s => dto.ShiftIds.Contains(s.Id)).ToList();
        model.StationCategories =
            dbContext.StationCategories.Where(sc => dto.StationCategoryIds.Contains(sc.Id)).ToList();
    }
}