using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;

namespace FastFuel.Features.Foods.Controllers;

public class FoodController(ICrudService<FoodRequestDto, FoodResponseDto> service)
    : CrudController<Food, FoodRequestDto, FoodResponseDto>(service);