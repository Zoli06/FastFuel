using FastFuel.Features.Common;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Mappers;
using FastFuel.Features.Menus.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Menus.Controllers;

public class MenuController(ApplicationDbContext dbContext)
    : ApplicationController<Menu, MenuRequestDto, MenuResponseDto>(dbContext)
{
    protected override Mapper<Menu, MenuRequestDto, MenuResponseDto> Mapper => new MenuMapper();
    protected override DbSet<Menu> DbSet => DbContext.Menus;
}