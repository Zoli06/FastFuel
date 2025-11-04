using AutoMapper;
using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;

namespace FastFuel.Features.Allergies.AutoMapperProfiles
{
    public class AllergyProfile : Profile
    {
        public AllergyProfile()
        {
            CreateMap<Allergy, AllergyDto>().ReverseMap();
            CreateMap<Allergy, EditAllergyDto>().ReverseMap();
        }
    }
}
