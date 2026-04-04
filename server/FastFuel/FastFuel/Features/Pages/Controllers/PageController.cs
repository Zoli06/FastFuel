using FastFuel.Features.Pages.DTOs;
using FastFuel.Features.Pages.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Pages.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PageController(IPageService pageService) : ControllerBase
{
    /// <summary>
    /// Gets static page permission metadata used by the frontend.
    /// </summary>
    /// <returns>All page permission requirements.</returns>
    [HttpGet]
    [AllowAnonymous]
    public Results<Ok<List<PagePermissionsResponseDto>>, UnauthorizedHttpResult> GetAll()
    {
        return TypedResults.Ok(pageService.GetAll());
    }
}