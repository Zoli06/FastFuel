using System.Security.Claims;
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
    protected ICrudService<TRequest, TResponse> Service { get; } = service;

    [HttpGet]
    [CrudAuthorize(PermissionType.Read)]
    public virtual async Task<Results<
            Ok<List<TResponse>>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetAll(CancellationToken cancellationToken = default)
    {
        var dtos = await Service.GetAllAsync(GetUserId(User), cancellationToken);
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
        var dto = await Service.GetByIdAsync(id, GetUserId(User), cancellationToken);
        if (dto == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(dto);
    }

    [HttpPost]
    [CrudAuthorize(PermissionType.Create)]
    public virtual async Task<Results<
            Created<TResponse>,
            Conflict<ProblemDetails>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        Create(TRequest requestDto, CancellationToken cancellationToken = default)
    {
        var responseDto = await Service.CreateAsync(requestDto, GetUserId(User), cancellationToken);
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
        var success = await Service.UpdateAsync(id, requestDto, GetUserId(User), cancellationToken);
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
        var success = await Service.DeleteAsync(id, GetUserId(User), cancellationToken);
        if (!success)
            return TypedResults.NotFound();
        return TypedResults.NoContent();
    }

    protected uint? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && uint.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }
}