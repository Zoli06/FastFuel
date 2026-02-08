using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Stations.Services;

public class StationService(
    ApplicationDbContext dbContext,
    IMapper<Station, StationRequestDto, StationResponseDto> mapper)
    : CrudService<Station, StationRequestDto, StationResponseDto>(dbContext, mapper)
{
    protected override DbSet<Station> DbSet { get; } = dbContext.Stations;
}