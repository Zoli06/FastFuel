using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Models;
using FastFuel.Features.Users.Mappers;
using FastFuel.Features.Users.Models;

namespace FastFuel.Features.Customers.Mappers;

public class CustomerMapper : UserMapper, IMapper<Customer, CustomerRequestDto, CustomerResponseDto>
{
    public CustomerResponseDto ToDto(Customer model)
    {
        return new CustomerResponseDto
        {
            Id = model.Id,
            Name = model.UserName,
            Email = model.Email,
            UserName = model.UserName,
            ThemeId = model.ThemeId,
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

    public void UpdateModel(CustomerRequestDto dto, ref Customer model)
    {
        User userModel = model;
        base.UpdateModel(dto, ref userModel);
    }
}