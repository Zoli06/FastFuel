using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Mappers;
using FastFuel.Features.Foods.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Foods.Services;

public class FoodService(ApplicationDbContext dbContext) : CrudService<Food, FoodRequestDto, FoodResponseDto>(dbContext)
{
    protected override Mapper<Food, FoodRequestDto, FoodResponseDto> Mapper => new FoodMapper();
    protected override DbSet<Food> DbSet => DbContext.Foods;
}