using FastFuel.Features.Common;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Mappers;
using FastFuel.Features.StationCategories.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.StationCategories.Controllers;

public class StationCategoryController(ApplicationDbContext dbContext)
    : ApplicationController<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto>(dbContext)
{
    protected override Mapper<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto> Mapper =>
        new StationCategoryMapper(DbContext);

    protected override DbSet<StationCategory> DbSet => DbContext.StationCategories;
}