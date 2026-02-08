using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Ingredients.Services;

public class IngredientService(
    ApplicationDbContext dbContext,
    IMapper<Ingredient, IngredientRequestDto, IngredientResponseDto> mapper)
    : CrudService<Ingredient, IngredientRequestDto, IngredientResponseDto>(dbContext, mapper)
{
    protected override DbSet<Ingredient> DbSet { get; } = dbContext.Ingredients;
}