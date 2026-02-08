using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Users.Services;

public class UserService(ApplicationDbContext dbContext, IMapper<User, UserRequestDto, UserResponseDto> mapper) : CrudService<User, UserRequestDto, UserResponseDto>(dbContext, mapper)
{
    protected override DbSet<User> DbSet { get; } = dbContext.Users;
}