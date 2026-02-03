using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Mappers;
using FastFuel.Features.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Users.Services;

public class UserService(ApplicationDbContext dbContext) : CrudService<User, UserRequestDto, UserResponseDto>(dbContext)
{
    protected override Mapper<User, UserRequestDto, UserResponseDto> Mapper { get; } = new UserMapper();
    protected override DbSet<User> DbSet { get; } = dbContext.Users;
}