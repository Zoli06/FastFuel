using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Models;

namespace FastFuel.Features.Users.Controllers;

public class UserController(ICrudService<UserRequestDto, UserResponseDto> service)
    : CrudController<User, UserRequestDto, UserResponseDto>(service);