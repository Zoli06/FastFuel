using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;

namespace FastFuel.Features.Menus.Controllers;

public class MenuController(ICrudService<MenuRequestDto, MenuResponseDto> service)
    : CrudController<Menu, MenuRequestDto, MenuResponseDto>(service);