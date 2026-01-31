using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Mappers;
using FastFuel.Features.Stations.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Stations.Services;

public class StationService(ApplicationDbContext dbContext) : CrudService<Station, StationRequestDto, StationResponseDto>(dbContext)
{
    protected override Mapper<Station, StationRequestDto, StationResponseDto> Mapper => new StationMapper();
    protected override DbSet<Station> DbSet => DbContext.Stations;
}