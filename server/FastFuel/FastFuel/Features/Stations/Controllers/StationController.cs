using FastFuel.Features.Common;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Mappers;
using FastFuel.Features.Stations.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Stations.Controllers;

public class StationController(ApplicationDbContext dbContext) : ApplicationController<Station, StationRequestDto, StationResponseDto>(dbContext)
{
    protected override Mapper<Station, StationRequestDto, StationResponseDto> Mapper => new StationMapper();
    protected override DbSet<Station> DbSet => DbContext.Stations;
}