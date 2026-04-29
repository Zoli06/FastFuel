using FastFuel.Features.Admins.DTOs;
using FastFuel.Features.Admins.Entities;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Mappers;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Admins.Mappers;

public class AdminMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : UserMapper(roleManager, userManager), IMapper<Admin, AdminRequestDto, AdminResponseDto>
{
    public AdminResponseDto ToDto(Admin entity)
    {
        var userDto = base.ToDto(entity);
        return new AdminResponseDto
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Email = entity.Email,
            UserName = userDto.UserName,
            RoleIds = userDto.RoleIds,
            UserType = userDto.UserType,
            OrderIds = userDto.OrderIds
        };
    }

    public Admin ToEntity(AdminRequestDto dto)
    {
        var userEntity = base.ToEntity(dto);
        return new Admin
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            Email = dto.Email,
            UserName = userEntity.UserName
        };
    }

    public void UpdateEntity(AdminRequestDto dto, Admin entity)
    {
        User userEntity = entity;
        base.UpdateEntity(dto, userEntity);
        entity.Email = dto.Email;
    }
}