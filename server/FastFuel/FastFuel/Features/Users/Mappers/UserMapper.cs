using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Mappers;

public abstract class UserMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : IMapper<User, UserRequestDto, UserResponseDto>
{
    protected abstract string UserType { get; }

    public virtual UserResponseDto ToDto(User model)
    {
        // TODO: make mappers async to avoid blocking calls to GetRolesAsync
        var userRoles = userManager.GetRolesAsync(model).Result;

        return new UserResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email,
            UserName = model.UserName,
            ThemeId = model.ThemeId,
            RoleIds = roleManager.Roles
                .Where(r => userRoles.Contains(r.Name))
                .Select(r => r.Id)
                .ToList(),
            UserType = UserType
        };
    }

    public User ToEntity(UserRequestDto dto)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            UserName = dto.UserName,
            ThemeId = dto.ThemeId
        };
    }

    public void UpdateEntity(UserRequestDto dto, User model)
    {
        model.Name = dto.Name;
        model.Email = dto.Email;
        model.UserName = dto.UserName;
        model.ThemeId = dto.ThemeId;
    }
}