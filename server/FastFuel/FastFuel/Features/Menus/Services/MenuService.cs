using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Mappers;
using FastFuel.Features.Menus.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Menus.Services;

public class MenuService(ApplicationDbContext dbContext) : CrudService<Menu, MenuRequestDto, MenuResponseDto>(dbContext)
{
    protected override Mapper<Menu, MenuRequestDto, MenuResponseDto> Mapper =>
        new MenuMapper();

    protected override DbSet<Menu> DbSet => DbContext.Menus;
}