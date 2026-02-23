using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Entities;

namespace FastFuel.Features.Stations.Mappers;

public class StationMapper : IMapper<Station, StationRequestDto, StationResponseDto>
{
    public StationResponseDto ToDto(Station entity)
    {
        return new StationResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            InOperation = entity.InOperation,
            RestaurantId = entity.RestaurantId,
            StationCategoryId = entity.StationCategoryId
        };
    }

    public Station ToEntity(StationRequestDto dto)
    {
        return new Station
        {
            Name = dto.Name,
            InOperation = dto.InOperation,
            RestaurantId = dto.RestaurantId,
            StationCategoryId = dto.StationCategoryId
        };
    }

    public void UpdateEntity(StationRequestDto dto, Station entity)
    {
        entity.Name = dto.Name;
        entity.InOperation = dto.InOperation;
        entity.RestaurantId = dto.RestaurantId;
        entity.StationCategoryId = dto.StationCategoryId;
    }
}