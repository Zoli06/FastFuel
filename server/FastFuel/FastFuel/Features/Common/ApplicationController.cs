using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public abstract class ApplicationController<TModel, TRequest, TResponse>(ApplicationDbContext dbContext)
    : ControllerBase where TModel : class, IIdentifiable where TResponse : IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected abstract Mapper<TModel, TRequest, TResponse> Mapper { get; }
    protected abstract DbSet<TModel> DbSet { get; }

    [HttpGet]
    public virtual async Task<Results<Ok<List<TResponse>>, UnauthorizedHttpResult>> GetAll()
    {
        var models = await DbSet.ToListAsync();
        return TypedResults.Ok(models.ConvertAll(m => Mapper.ToDto(m)));
    }

    [HttpGet("{id:int}")]
    public virtual async Task<Results<Ok<TResponse>, NotFound, UnauthorizedHttpResult>> GetById(uint id)
    {
        var model = await DbSet.FindAsync(id);
        if (model == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(Mapper.ToDto(model));
    }

    [HttpPost]
    public virtual async Task<Results<Created<TResponse>, Conflict<ProblemDetails>, UnauthorizedHttpResult>> Create(TRequest requestDto)
    {
        var model = Mapper.ToModel(requestDto);
        DbSet.Add(model);
        await DbContext.SaveChangesAsync();
        var responseDto = Mapper.ToDto(model);
        var location = Url.Action(nameof(GetById), new { id = responseDto.Id });
        return TypedResults.Created(location!, responseDto);
    }

    [HttpPut("{id:int}")]
    public virtual async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>, Conflict<ProblemDetails>, UnauthorizedHttpResult>> Update(uint id, TRequest requestDto)
    {
        var model = await DbSet
            .FirstOrDefaultAsync(a => a.Id == id);

        if (model == null)
            return TypedResults.NotFound();

        Mapper.UpdateModel(requestDto, ref model);
        await DbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>, UnauthorizedHttpResult>> Delete(uint id)
    {
        var model = await DbSet.FindAsync(id);
        if (model == null)
            return TypedResults.NotFound();
        DbSet.Remove(model);
        await DbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}