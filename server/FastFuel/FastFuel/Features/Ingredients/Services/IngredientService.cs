using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Mappers;
using FastFuel.Features.Ingredients.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Ingredients.Services;

public class IngredientService(ApplicationDbContext dbContext) : CrudService<Ingredient, IngredientRequestDto, IngredientResponseDto>(dbContext)
{
    protected override Mapper<Ingredient, IngredientRequestDto, IngredientResponseDto> Mapper =>
        new IngredientMapper(DbContext);
    protected override DbSet<Ingredient> DbSet => DbContext.Ingredients;
}