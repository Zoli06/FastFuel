using FastFuel.Features.Common;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Mappers;
using FastFuel.Features.Ingredients.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Ingredients.Controllers;

public class FoodController(ApplicationDbContext dbContext)
    : ApplicationController<Ingredient, IngredientRequestDto, IngredientResponseDto>(dbContext)
{
    protected override Mapper<Ingredient, IngredientRequestDto, IngredientResponseDto> Mapper =>
        new IngredientMapper(DbContext);

    protected override DbSet<Ingredient> DbSet => DbContext.Ingredients;
}