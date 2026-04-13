using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Restaurants.Controllers;

public class RestaurantController(ICrudService<RestaurantRequestDto, RestaurantResponseDto> service)
    : CrudController<Restaurant, RestaurantRequestDto, RestaurantResponseDto>(service)
{
    [AllowAnonymous]
    public override Task<Results<Ok<List<RestaurantResponseDto>>, BadRequest<ProblemDetails>, UnauthorizedHttpResult, ForbidHttpResult>> GetAll(CancellationToken cancellationToken = default)
    {
        return base.GetAll(cancellationToken);
    }

    [AllowAnonymous]
    public override Task<Results<Ok<RestaurantResponseDto>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> GetById(uint id, CancellationToken cancellationToken = default)
    {
        return base.GetById(id, cancellationToken);
    }
}