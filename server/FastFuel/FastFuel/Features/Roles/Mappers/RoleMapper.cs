using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Models;
using FastFuel.Features.Users.Models;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Roles.Mappers;

public class RoleMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : IMapper<Role, RoleRequestDto, RoleResponseDto>
{
    public RoleResponseDto ToDto(Role model)
    {
        return new RoleResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Permissions = roleManager.GetClaimsAsync(new Role { Id = model.Id, Name = model.Name }).Result
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList(),
            UserIds = userManager.GetUsersInRoleAsync(model.Name).Result.Select(u => u.Id).ToList()
        };
    }

    public Role ToModel(RoleRequestDto dto)
    {
        return new Role
        {
            Name = dto.Name
        };
    }

    public void UpdateModel(RoleRequestDto dto, Role model)
    {
        model.Name = dto.Name;
    }
}