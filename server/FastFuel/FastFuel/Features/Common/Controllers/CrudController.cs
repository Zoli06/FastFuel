using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Common.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public abstract class CrudController<TModel, TRequest, TResponse>(ICrudService<TRequest, TResponse> service)
    : ControllerBase where TModel : class, IIdentifiable where TResponse : IIdentifiable
{
    [HttpGet]
    public virtual async Task<Results<Ok<List<TResponse>>, UnauthorizedHttpResult>> GetAll()
    {
        var dtos = await service.GetAllAsync();
        return TypedResults.Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public virtual async Task<Results<Ok<TResponse>, NotFound, UnauthorizedHttpResult>> GetById(uint id)
    {
        var dto = await service.GetByIdAsync(id);
        if (dto == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(dto);
    }

    [HttpPost]
    public virtual async Task<Results<Created<TResponse>, Conflict<ProblemDetails>, UnauthorizedHttpResult>> Create(
        TRequest requestDto)
    {
        var responseDto = await service.CreateAsync(requestDto);
        var location = Url.Action(nameof(GetById), new { id = responseDto.Id });
        return TypedResults.Created(location!, responseDto);
    }

    [HttpPut("{id:int}")]
    public virtual async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>, Conflict<ProblemDetails>,
        UnauthorizedHttpResult>> Update(
        uint id, TRequest requestDto)
    {
        var success = await service.UpdateAsync(id, requestDto);
        if (!success)
            return TypedResults.NotFound();
        return TypedResults.NoContent();
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>, UnauthorizedHttpResult>>
        Delete(uint id)
    {
        var success = await service.DeleteAsync(id);
        if (!success)
            return TypedResults.NotFound();
        return TypedResults.NoContent();
    }
}