using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Mappers;

public abstract class UserMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
{
    private static string GetRuntimeUserType(User model)
    {
        var type = model.GetType();

        if (type == typeof(User))
            return nameof(User);

        // EF proxy instances inherit from the concrete entity type.
        if (type.Name.EndsWith("Proxy", StringComparison.Ordinal)
            && type.BaseType is not null
            && typeof(User).IsAssignableFrom(type.BaseType))
            return type.BaseType.Name;

        return type.Name;
    }

    public virtual UserResponseDto ToDto(User model)
    {
        // TODO: make mappers async to avoid blocking calls to GetRolesAsync
        var userRoles = userManager.GetRolesAsync(model).Result;

        return new UserResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            UserName = model.UserName,
            RoleIds = roleManager.Roles
                .Where(r => userRoles.Contains(r.Name))
                .Select(r => r.Id)
                .ToList(),
            UserType = GetRuntimeUserType(model),
            OrderIds = model.Orders.Select(o => o.Id).ToList()
        };
    }

    public User ToEntity(UserRequestDto dto)
    {
        return new User
        {
            Name = dto.Name,
            UserName = dto.UserName
        };
    }

    public void UpdateEntity(UserRequestDto dto, User model)
    {
        model.Name = dto.Name;
        model.UserName = dto.UserName;
    }
}