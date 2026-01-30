using FastFuel.Features.Common;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Mappers;
using FastFuel.Features.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Users.Controllers;

public class UserController(ApplicationDbContext dbContext)
    : ApplicationController<User, UserRequestDto, UserResponseDto>(dbContext)
{
    protected override Mapper<User, UserRequestDto, UserResponseDto> Mapper => new UserMapper();
    protected override DbSet<User> DbSet => DbContext.Users;
}