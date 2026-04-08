using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Mappers;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Customers.Mappers;

public class CustomerMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : UserMapper(roleManager, userManager), IMapper<Customer, CustomerRequestDto, CustomerResponseDto>
{
    public CustomerResponseDto ToDto(Customer entity)
    {
        var userDto = base.ToDto(entity);
        return new CustomerResponseDto
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Email = entity.Email,
            UserName = userDto.UserName,
            RoleIds = userDto.RoleIds,
            UserType = userDto.UserType,
            OrderIds = entity.Orders.ConvertAll(order => order.Id)
        };
    }

    public Customer ToEntity(CustomerRequestDto dto)
    {
        var userEntity = base.ToEntity(dto);
        return new Customer
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            Email = dto.Email,
            UserName = userEntity.UserName
        };
    }

    public void UpdateEntity(CustomerRequestDto dto, Customer entity)
    {
        User userEntity = entity;
        base.UpdateEntity(dto, userEntity);
        entity.Email = dto.Email;
    }
}