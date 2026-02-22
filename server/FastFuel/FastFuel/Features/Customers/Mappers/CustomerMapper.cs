using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Models;
using FastFuel.Features.Roles.Models;
using FastFuel.Features.Users.Mappers;
using FastFuel.Features.Users.Models;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Customers.Mappers;

public class CustomerMapper(RoleManager<Role> roleManager, UserManager<User> userManager)
    : UserMapper(roleManager, userManager), IMapper<Customer, CustomerRequestDto, CustomerResponseDto>
{
    protected override string UserType => "Customer";

    public CustomerResponseDto ToDto(Customer model)
    {
        var userDto = base.ToDto(model);
        return new CustomerResponseDto
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Email = userDto.Email,
            UserName = userDto.UserName,
            ThemeId = userDto.ThemeId,
            RoleIds = userDto.RoleIds,
            UserType = userDto.UserType,
            OrderIds = model.Orders.ConvertAll(order => order.Id)
        };
    }

    public Customer ToModel(CustomerRequestDto dto)
    {
        var userModel = base.ToModel(dto);
        return new Customer
        {
            Id = userModel.Id,
            Name = userModel.Name,
            Email = userModel.Email,
            UserName = userModel.UserName,
            ThemeId = userModel.ThemeId
        };
    }

    public void UpdateModel(CustomerRequestDto dto, Customer model)
    {
        User userModel = model;
        base.UpdateModel(dto, userModel);
    }
}