using AutoMapper;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.Ingredients.AutoMapperProfiles
{
    public class IngredientsProfile : Profile
    {
        public IngredientsProfile()
        {
            CreateMap<Ingredient, IngredientDto>().ReverseMap();
            CreateMap<Ingredient, EditIngredientDto>().ReverseMap();
        }
    }
}
