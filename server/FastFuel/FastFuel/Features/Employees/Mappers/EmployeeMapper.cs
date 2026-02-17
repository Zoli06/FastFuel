using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Models;

namespace FastFuel.Features.Employees.Mappers;

public class EmployeeMapper : IMapper<Employee, EmployeeRequestDto, EmployeeResponseDto>
{
    public EmployeeResponseDto ToDto(Employee model)
    {
        return new EmployeeResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email,
            Username = model.Username,
            ThemeId = model.Theme.Id
        };
    }

    public Employee ToModel(EmployeeRequestDto dto)
    {
        return new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            Username = dto.Username,
            ThemeId = dto.ThemeId
        };
    }

    public void UpdateModel(EmployeeRequestDto dto, ref Employee model)
    {
        model.Name = dto.Name;
        model.ThemeId = dto.ThemeId;
        model.Username = dto.Username;
        model.Email = dto.Email;
    }
}