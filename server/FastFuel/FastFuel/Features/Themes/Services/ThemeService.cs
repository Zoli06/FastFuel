using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Themes.Services;

public class ThemeService(ApplicationDbContext dbContext, IMapper<Theme, ThemeRequestDto, ThemeResponseDto> mapper) : CrudService<Theme, ThemeRequestDto, ThemeResponseDto>(dbContext, mapper)
{
    protected override DbSet<Theme> DbSet { get; } = dbContext.Themes;
}