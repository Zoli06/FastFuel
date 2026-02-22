using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Models;

namespace FastFuel.Features.Customers.Mappers;

public class CustomerMapper : IMapper<Customer, CustomerRequestDto, CustomerResponseDto>
{
    public CustomerResponseDto ToDto(Customer model)
    {
        return new CustomerResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email,
            Username = model.Username,
            ThemeId = model.Theme.Id
        };
    }

    public Customer ToModel(CustomerRequestDto dto)
    {
        return new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Username = dto.Username,
            ThemeId = dto.ThemeId
        };
    }

    public void UpdateModel(CustomerRequestDto dto, ref Customer model)
    {
        model.Name = dto.Name;
        model.ThemeId = dto.ThemeId;
        model.Username = dto.Username;
        model.Email = dto.Email;
    }
}