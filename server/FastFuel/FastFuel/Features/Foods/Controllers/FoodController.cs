using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;
using FastFuel.Features.Foods.Services;

namespace FastFuel.Features.Foods.Controllers;

public class FoodController(ApplicationDbContext dbContext)
    : CrudController<Food, FoodRequestDto, FoodResponseDto>
{
    protected override CrudService<Food, FoodRequestDto, FoodResponseDto> Service { get; } = new FoodService(dbContext);
}