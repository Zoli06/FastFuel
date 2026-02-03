using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Models;
using FastFuel.Features.Stations.Services;

namespace FastFuel.Features.Stations.Controllers;

public class StationController(ApplicationDbContext dbContext)
    : CrudController<Station, StationRequestDto, StationResponseDto>
{
    protected override CrudService<Station, StationRequestDto, StationResponseDto> Service =>
        new StationService(dbContext);
}