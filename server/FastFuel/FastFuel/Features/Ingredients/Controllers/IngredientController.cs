using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;
using FastFuel.Features.Ingredients.Services;

namespace FastFuel.Features.Ingredients.Controllers;

public class IngredientController(ApplicationDbContext dbContext)
    : CrudController<Ingredient, IngredientRequestDto, IngredientResponseDto>
{
    protected override CrudService<Ingredient, IngredientRequestDto, IngredientResponseDto> Service { get; } =
        new IngredientService(dbContext);
}