using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Mappers;
using FastFuel.Features.StationCategories.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.StationCategories.Services;

public class StationCategoryService(ApplicationDbContext dbContext) : CrudService<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto>(dbContext)
{
    protected override Mapper<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto> Mapper => new StationCategoryMapper(DbContext);
    protected override DbSet<StationCategory> DbSet => DbContext.StationCategories;
}