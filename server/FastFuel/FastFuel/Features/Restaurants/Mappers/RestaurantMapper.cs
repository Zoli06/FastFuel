using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OpeningHours.Entities;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Entities;

namespace FastFuel.Features.Restaurants.Mappers;

public class RestaurantMapper : IMapper<Restaurant, RestaurantRequestDto, RestaurantResponseDto>
{
    public RestaurantResponseDto ToDto(Restaurant entity)
    {
        return new RestaurantResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Address = entity.Address,
            Phone = entity.Phone,
            OpeningHours = entity.OpeningHours.ConvertAll(ToDto)
        };
    }

    public Restaurant ToEntity(RestaurantRequestDto dto)
    {
        return new Restaurant
        {
            Name = dto.Name,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Address = dto.Address,
            Phone = dto.Phone,
            OpeningHours = dto.OpeningHours.ConvertAll(ToEntity)
        };
    }

    public void UpdateEntity(RestaurantRequestDto dto, Restaurant entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Latitude = dto.Latitude;
        entity.Longitude = dto.Longitude;
        entity.Address = dto.Address;
        entity.Phone = dto.Phone;

        entity.OpeningHours.Clear();
        entity.OpeningHours.AddRange(dto.OpeningHours.ConvertAll(ToEntity));
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

    private OpeningHour ToEntity(RestaurantOpeningHourDto dto)
    {
        return new OpeningHour
        {
            DayOfWeek = dto.DayOfWeek,
            OpenTime = dto.OpenTime,
            CloseTime = dto.CloseTime
        };
    }
}