using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.StationCategories.Services;

public class StationCategoryService(
    ApplicationDbContext dbContext,
    IMapper<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto> mapper)
    : CrudService<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto>(dbContext, mapper)
{
    protected override DbSet<StationCategory> DbSet { get; } = dbContext.StationCategories;
}