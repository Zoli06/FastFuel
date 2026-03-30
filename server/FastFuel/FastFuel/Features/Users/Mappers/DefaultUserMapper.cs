using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Mappers;

public class DefaultUserMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : UserMapper(roleManager, userManager), IMapper<User, UserRequestDto, UserResponseDto>;