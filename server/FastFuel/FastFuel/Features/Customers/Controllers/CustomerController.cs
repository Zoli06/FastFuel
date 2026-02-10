using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Models;

namespace FastFuel.Features.Customers.Controllers;

public class CustomerController(ICrudService<CustomerRequestDto, CustomerResponseDto> service)
    : CrudController<Customer, CustomerRequestDto, CustomerResponseDto>(service);