using AutoMapper;
using FastFuel.Features.OpeningHours.DTOs;
using FastFuel.Features.OpeningHours.Models;

namespace FastFuel.Features.OpeningHours.AutoMapperProfiles
{
    public class OpeningHourProfile : Profile
    {
        public OpeningHourProfile()
        {
            CreateMap<OpeningHour, OpeningHourDto>().ReverseMap();
            CreateMap<OpeningHour, EditOpeningHourDto>().ReverseMap();
        }
    }
}

