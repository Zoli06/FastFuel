namespace FastFuel.Features.Stations.DTOs;

public record StationTasksResponseDto
{
    /// <summary>
    /// The list of orders.
    /// </summary>
    public required List<StationTaskOrder> Orders { get; init; }
}