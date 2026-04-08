using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Machines.DTOs;
using FastFuel.Features.Machines.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Mappers;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Machines.Mappers;

public class MachineMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : UserMapper(roleManager, userManager), IMapper<Machine, MachineRequestDto, MachineResponseDto>
{
    public MachineResponseDto ToDto(Machine entity)
    {
        var userDto = base.ToDto(entity);
        return new MachineResponseDto
        {
            Id = userDto.Id,
            Name = userDto.Name,
            UserName = userDto.UserName,
            RoleIds = userDto.RoleIds,
            UserType = userDto.UserType,
            LocatedAtRestaurantId = entity.LocatedAtRestaurantId,
            OrderIds = entity.Orders.ConvertAll(order => order.Id)
        };
    }

    public Machine ToEntity(MachineRequestDto dto)
    {
        var userEntity = base.ToEntity(dto);
        return new Machine
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            UserName = userEntity.UserName,
            LocatedAtRestaurantId = dto.LocatedAtRestaurantId
        };
    }

    public void UpdateEntity(MachineRequestDto dto, Machine entity)
    {
        User userEntity = entity;
        base.UpdateEntity(dto, userEntity);

        entity.LocatedAtRestaurantId = dto.LocatedAtRestaurantId;
    }
}