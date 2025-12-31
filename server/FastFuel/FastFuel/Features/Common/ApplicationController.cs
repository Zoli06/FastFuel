using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common;

[ApiController]
[Route("/api/[controller]")]
public abstract class ApplicationController<TModel, TRequest, TResponse>(ApplicationDbContext dbContext)
    : ControllerBase where TModel : class, IIdentifiable where TResponse : IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected abstract Mapper<TModel, TRequest, TResponse> Mapper { get; }
    protected abstract DbSet<TModel> DbSet { get; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var models = await DbSet.ToListAsync();
        return Ok(models.ConvertAll(m => Mapper.ToDto(m)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(uint id)
    {
        var model = await DbSet.FindAsync(id);
        if (model == null)
            return NotFound();
        return Ok(Mapper.ToDto(model));
    }

    [HttpPost]
    public async Task<IActionResult> Create(TRequest requestDto)
    {
        var model = Mapper.ToModel(requestDto);
        DbSet.Add(model);
        await DbContext.SaveChangesAsync();
        var responseDto = Mapper.ToDto(model);
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, responseDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(uint id, TRequest requestDto)
    {
        var model = await DbSet
            .FirstOrDefaultAsync(a => a.Id == id);

        if (model == null)
            return NotFound();

        Mapper.UpdateModel(requestDto, ref model);
        await DbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(uint id)
    {
        var model = await DbSet.FindAsync(id);
        if (model == null)
            return NotFound();
        DbSet.Remove(model);
        await DbContext.SaveChangesAsync();
        return NoContent();
    }
}