using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;
using FastFuel.Features.StationCategories.Services;

namespace FastFuel.Features.StationCategories.Controllers;

public class StationCategoryController(ApplicationDbContext dbContext)
    : CrudController<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto>
{
    protected override CrudService<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto> Service =>
        new StationCategoryService(dbContext);
}