using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Models;

namespace FastFuel.Features.Roles.Controllers;

public class RoleController(ICrudService<RoleRequestDto, RoleResponseDto> service)
    : CrudController<Role, RoleRequestDto, RoleResponseDto>(service);