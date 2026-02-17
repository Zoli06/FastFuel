using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Models;

namespace FastFuel.Features.Employees.Controllers;

public class EmployeeController(ICrudService<EmployeeRequestDto, EmployeeResponseDto> service)
    : CrudController<Employee, EmployeeRequestDto, EmployeeResponseDto>(service);