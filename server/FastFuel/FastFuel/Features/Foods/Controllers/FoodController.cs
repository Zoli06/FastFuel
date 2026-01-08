using FastFuel.Features.Common;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Mappers;
using FastFuel.Features.Foods.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Foods.Controllers;

public class FoodController(ApplicationDbContext dbContext)
    : ApplicationController<Food, FoodRequestDto, FoodResponseDto>(dbContext)
{
    protected override Mapper<Food, FoodRequestDto, FoodResponseDto> Mapper => new FoodMapper();
    protected override DbSet<Food> DbSet => DbContext.Foods;
}