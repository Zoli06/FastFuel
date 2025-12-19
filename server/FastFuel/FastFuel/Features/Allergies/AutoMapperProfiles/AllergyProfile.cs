using AutoMapper;
using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;

namespace FastFuel.Features.Allergies.AutoMapperProfiles;

public class AllergyProfile : Profile
{
    public AllergyProfile()
    {
        CreateMap<Allergy, AllergyDto>()
            .ForMember(dest => dest.IngredientIds, opt => opt.MapFrom(src => src.Ingredients.ConvertAll(i => i.Id)));
        
        CreateMap<EditAllergyDto, Allergy>();
    }
}