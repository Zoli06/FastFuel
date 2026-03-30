using FastFuel.Features.Pages.DTOs;

namespace FastFuel.Features.Pages.Services;

public interface IPageService
{
    List<PagePermissionsResponseDto> GetAll();
}