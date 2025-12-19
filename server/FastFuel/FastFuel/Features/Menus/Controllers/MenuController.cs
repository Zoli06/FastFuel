using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Menus.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class MenuController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuDto>>> GetMenus()
    {
        var menus = await context.Menus.ToListAsync();
        return Ok(mapper.Map<List<MenuDto>>(menus));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MenuDto>> GetMenu(uint id)
    {
        var menu = await context.Menus.FindAsync(id);
        if (menu == null)
            return NotFound();

        return Ok(mapper.Map<MenuDto>(menu));
    }

    [HttpPost]
    public async Task<ActionResult<MenuDto>> CreateMenu(EditMenuDto menuDto)
    {
        var menu = mapper.Map<Menu>(menuDto);
        context.Menus.Add(menu);
        await context.SaveChangesAsync();

        var createdMenuDto = mapper.Map<MenuDto>(menu);
        return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, createdMenuDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMenu(uint id, EditMenuDto menuDto)
    {
        var menu = await context.Menus.FindAsync(id);
        if (menu == null)
            return NotFound();

        mapper.Map(menuDto, menu);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMenu(uint id)
    {
        var menu = await context.Menus.FindAsync(id);
        if (menu == null)
            return NotFound();

        context.Menus.Remove(menu);
        await context.SaveChangesAsync();

        return NoContent();
    }
}