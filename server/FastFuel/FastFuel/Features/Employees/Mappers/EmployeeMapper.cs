using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Mappers;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Employees.Mappers;

public class EmployeeMapper(
    ApplicationDbContext dbContext,
    RoleManager<Role> roleManager,
    UserManager<User> userManager)
    : UserMapper(roleManager, userManager), IMapper<Employee, EmployeeRequestDto, EmployeeResponseDto>
{
    protected override string UserType => "Employee";

    public EmployeeResponseDto ToDto(Employee entity)
    {
        var userDto = base.ToDto(entity);
        return new EmployeeResponseDto
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Email = userDto.Email,
            UserName = userDto.UserName,
            ThemeId = userDto.ThemeId,
            RoleIds = userDto.RoleIds,
            UserType = userDto.UserType,
            ShiftIds = entity.Shifts.ConvertAll(shift => shift.Id),
            StationCategoryIds = entity.StationCategories.ConvertAll(category => category.Id)
        };
    }

    public Employee ToEntity(EmployeeRequestDto dto)
    {
        var userEntity = base.ToEntity(dto);
        return new Employee
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            Email = userEntity.Email,
            UserName = userEntity.UserName,
            ThemeId = userEntity.ThemeId,
            Shifts = dbContext.Shifts.Where(s => dto.ShiftIds.Contains(s.Id)).ToList(),
            StationCategories = dbContext.StationCategories.Where(sc => dto.StationCategoryIds.Contains(sc.Id)).ToList()
        };
    }

    public void UpdateEntity(EmployeeRequestDto dto, Employee entity)
    {
        User userEntity = entity;
        base.UpdateEntity(dto, userEntity);
        entity.Shifts = dbContext.Shifts.Where(s => dto.ShiftIds.Contains(s.Id)).ToList();
        entity.StationCategories =
            dbContext.StationCategories.Where(sc => dto.StationCategoryIds.Contains(sc.Id)).ToList();
    }
}