using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Entities;

namespace FastFuel.Features.Stations.Controllers;

public class StationController(ICrudService<StationRequestDto, StationResponseDto> service)
    : CrudController<Station, StationRequestDto, StationResponseDto>(service);