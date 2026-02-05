using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.StationCategories.Controllers;

public class StationCategoryController(ICrudService<StationCategoryRequestDto, StationCategoryResponseDto> service)
    : CrudController<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto>(service);