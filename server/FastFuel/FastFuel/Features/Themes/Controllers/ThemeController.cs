using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Models;

namespace FastFuel.Features.Themes.Controllers;

public class ThemeController(ICrudService<ThemeRequestDto, ThemeResponseDto> service)
    : CrudController<Theme, ThemeRequestDto, ThemeResponseDto>(service);