using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Models;
using FastFuel.Features.Themes.Services;

namespace FastFuel.Features.Themes.Controllers;

public class ThemeController(ApplicationDbContext dbContext)
    : CrudController<Theme, ThemeRequestDto, ThemeResponseDto>
{
    protected override CrudService<Theme, ThemeRequestDto, ThemeResponseDto> Service { get; } = new ThemeService(dbContext);
}