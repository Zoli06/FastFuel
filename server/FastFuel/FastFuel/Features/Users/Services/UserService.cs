using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Users.Services;

public class UserService(
    ApplicationDbContext dbContext,
    IMapper<User, UserRequestDto, UserResponseDto> mapper,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
    : UserServiceBase<User, UserRequestDto, UserResponseDto>(dbContext, mapper, userManager, roleManager)
{
    protected override DbSet<User> DbSet { get; } = dbContext.Users;
}