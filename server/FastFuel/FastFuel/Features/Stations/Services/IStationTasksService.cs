using FastFuel.Features.Common.Services;
using FastFuel.Features.Stations.DTOs;

namespace FastFuel.Features.Stations.Services;

public interface IStationService : ICrudService<StationRequestDto, StationResponseDto>
{
    Task<List<StationResponseDto>> GetAllStationsWithFiltersAsync(uint? restaurantId,
        CancellationToken cancellationToken = default);

    Task<StationTasksResponseDto?> GetTasksForStationAsync(uint stationId,
        CancellationToken cancellationToken = default);
}