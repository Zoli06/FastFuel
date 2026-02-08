using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Menus.Services;

public class MenuService(ApplicationDbContext dbContext, IMapper<Menu, MenuRequestDto, MenuResponseDto> mapper)
    : CrudService<Menu, MenuRequestDto, MenuResponseDto>(dbContext, mapper)
{
    protected override DbSet<Menu> DbSet { get; } = dbContext.Menus;
}