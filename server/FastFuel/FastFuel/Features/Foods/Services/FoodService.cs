using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Foods.Services;

public class FoodService(ApplicationDbContext dbContext, IMapper<Food, FoodRequestDto, FoodResponseDto> mapper)
    : CrudService<Food, FoodRequestDto, FoodResponseDto>(dbContext, mapper)
{
    protected override DbSet<Food> DbSet { get; } = dbContext.Foods;
}