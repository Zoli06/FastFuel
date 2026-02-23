using FastFuel.Features.Common.Authorization;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Common.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public abstract class CrudController<TEntity, TRequest, TResponse>(ICrudService<TRequest, TResponse> service)
    : ControllerBase where TEntity : class, IIdentifiable where TResponse : IIdentifiable
{
    [HttpGet]
    [CrudAuthorize(PermissionType.Read)]
    public virtual async Task<Results<
            Ok<List<TResponse>>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetAll(CancellationToken cancellationToken = default)
    {
        var dtos = await service.GetAllAsync(cancellationToken);
        return TypedResults.Ok(dtos);
    }

    [HttpGet("{id:int}")]
    [CrudAuthorize(PermissionType.Read)]
    public virtual async Task<Results<
            Ok<TResponse>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetById(uint id, CancellationToken cancellationToken = default)
    {
        var dto = await service.GetByIdAsync(id, cancellationToken);
        if (dto == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(dto);
    }

    [HttpPost]
    [CrudAuthorize(PermissionType.Create)]
    public virtual async Task<Results<
            Created<TResponse>,
            Conflict<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        Create(TRequest requestDto, CancellationToken cancellationToken = default)
    {
        var responseDto = await service.CreateAsync(requestDto, cancellationToken);
        var location = Url.Action(nameof(GetById), new { id = responseDto.Id });
        return TypedResults.Created(location!, responseDto);
    }

    [HttpPut("{id:int}")]
    [CrudAuthorize(PermissionType.Update)]
    public virtual async Task<Results<
            NoContent,
            NotFound,
            BadRequest<ProblemDetails>,
            Conflict<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        Update(uint id, TRequest requestDto, CancellationToken cancellationToken = default)
    {
        var success = await service.UpdateAsync(id, requestDto, cancellationToken);
        if (!success)
            return TypedResults.NotFound();
        return TypedResults.NoContent();
    }

    [HttpDelete("{id:int}")]
    [CrudAuthorize(PermissionType.Delete)]
    public virtual async Task<Results<
            NoContent,
            NotFound,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        Delete(uint id, CancellationToken cancellationToken = default)
    {
        var success = await service.DeleteAsync(id, cancellationToken);
        if (!success)
            return TypedResults.NotFound();
        return TypedResults.NoContent();
    }
}