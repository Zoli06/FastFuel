using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OpeningHours.Models;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.Restaurants.Mappers;

public class RestaurantMapper : IMapper<Restaurant, RestaurantRequestDto, RestaurantResponseDto>
{
    public RestaurantResponseDto ToDto(Restaurant model)
    {
        return new RestaurantResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            Address = model.Address,
            Phone = model.Phone,
            OpeningHours = model.OpeningHours.ConvertAll(ToDto)
        };
    }

    public Restaurant ToModel(RestaurantRequestDto dto)
    {
        return new Restaurant
        {
            Name = dto.Name,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Address = dto.Address,
            Phone = dto.Phone,
            OpeningHours = dto.OpeningHours.ConvertAll(ToModel)
        };
    }

    public void UpdateModel(RestaurantRequestDto dto, ref Restaurant model)
    {
        model.Name = dto.Name;
        model.Description = dto.Description;
        model.Latitude = dto.Latitude;
        model.Longitude = dto.Longitude;
        model.Address = dto.Address;
        model.Phone = dto.Phone;

        model.OpeningHours.Clear();
        model.OpeningHours.AddRange(dto.OpeningHours.ConvertAll(ToModel));
    }

    private RestaurantOpeningHourDto ToDto(OpeningHour openingHour)
    {
        return new RestaurantOpeningHourDto
        {
            DayOfWeek = openingHour.DayOfWeek,
            OpenTime = openingHour.OpenTime,
            CloseTime = openingHour.CloseTime
        };
    }

    private OpeningHour ToModel(RestaurantOpeningHourDto dto)
    {
        return new OpeningHour
        {
            DayOfWeek = dto.DayOfWeek,
            OpenTime = dto.OpenTime,
            CloseTime = dto.CloseTime
        };
    }
}