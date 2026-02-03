using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Models;
using FastFuel.Features.Users.Services;

namespace FastFuel.Features.Users.Controllers;

public class UserController(ApplicationDbContext dbContext)
    : CrudController<User, UserRequestDto, UserResponseDto>
{
    protected override CrudService<User, UserRequestDto, UserResponseDto> Service { get; } = new UserService(dbContext);
}