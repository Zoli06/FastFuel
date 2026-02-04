using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Mappers;
using FastFuel.Features.Themes.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Themes.Services;

public class ThemeService(ApplicationDbContext dbContext) : CrudService<Theme, ThemeRequestDto, ThemeResponseDto>(dbContext)
{
    protected override Mapper<Theme, ThemeRequestDto, ThemeResponseDto> Mapper { get; } = new ThemeMapper();
    protected override DbSet<Theme> DbSet { get; } = dbContext.Themes;
}