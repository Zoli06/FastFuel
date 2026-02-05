using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.Ingredients.Controllers;

public class IngredientController(ICrudService<IngredientRequestDto, IngredientResponseDto> service)
    : CrudController<Ingredient, IngredientRequestDto, IngredientResponseDto>(service);