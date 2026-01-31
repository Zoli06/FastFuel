using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;
using FastFuel.Features.Menus.Services;

namespace FastFuel.Features.Menus.Controllers;

public class MenuController(ApplicationDbContext dbContext)
    : CrudController<Menu, MenuRequestDto, MenuResponseDto>
{
    protected override CrudService<Menu, MenuRequestDto, MenuResponseDto> Service =>
        new MenuService(dbContext);
}