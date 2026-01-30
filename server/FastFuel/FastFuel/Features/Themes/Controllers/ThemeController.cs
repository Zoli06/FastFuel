using FastFuel.Features.Common;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Mappers;
using FastFuel.Features.Themes.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Themes.Controllers;

public class ThemeController(ApplicationDbContext dbContext)
    : ApplicationController<Theme, ThemeRequestDto, ThemeResponseDto>(dbContext)
{
    protected override Mapper<Theme, ThemeRequestDto, ThemeResponseDto> Mapper => new ThemeMapper();
    protected override DbSet<Theme> DbSet => DbContext.Themes;
}