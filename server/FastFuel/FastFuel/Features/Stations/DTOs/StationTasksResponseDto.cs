namespace FastFuel.Features.Stations.DTOs;

public record StationTasksResponseDto
{
    public required List<StationTaskOrder> Orders { get; init; }
}