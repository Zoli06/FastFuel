using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Models;

namespace FastFuel.Features.Stations.Mappers;

public class StationMapper : IMapper<Station, StationRequestDto, StationResponseDto>
{
    public StationResponseDto ToDto(Station model)
    {
        return new StationResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            InOperation = model.InOperation,
            RestaurantId = model.RestaurantId,
            StationCategoryId = model.StationCategoryId
        };
    }

    public Station ToModel(StationRequestDto dto)
    {
        return new Station
        {
            Name = dto.Name,
            InOperation = dto.InOperation,
            RestaurantId = dto.RestaurantId,
            StationCategoryId = dto.StationCategoryId
        };
    }

    public void UpdateModel(StationRequestDto dto, ref Station model)
    {
        model.Name = dto.Name;
        model.InOperation = dto.InOperation;
        model.RestaurantId = dto.RestaurantId;
        model.StationCategoryId = dto.StationCategoryId;
    }
}