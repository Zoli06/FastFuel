using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Roles.Mappers;

public class RoleMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : IMapper<Role, RoleRequestDto, RoleResponseDto>
{
    public RoleResponseDto ToDto(Role entity)
    {
        return new RoleResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsDefault = entity.IsDefault,
            Permissions = roleManager.GetClaimsAsync(new Role { Id = entity.Id, Name = entity.Name }).Result
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList(),
            UserIds = userManager.GetUsersInRoleAsync(entity.Name).Result.Select(u => u.Id).ToList()
        };
    }

    public Role ToEntity(RoleRequestDto dto)
    {
        return new Role
        {
            Name = dto.Name
        };
    }

    public void UpdateEntity(RoleRequestDto dto, Role entity)
    {
        entity.Name = dto.Name;
    }
}