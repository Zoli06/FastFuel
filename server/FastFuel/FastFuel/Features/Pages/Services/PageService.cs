using FastFuel.Features.Pages.Common;
using FastFuel.Features.Pages.DTOs;

namespace FastFuel.Features.Pages.Services;

public class PageService : IPageService
{
    public List<PagePermissionsResponseDto> GetAll()
    {
        return Enum.GetValues<Page>()
            .Select(page =>
            {
                var permissions = PagePermissionCatalog.PagePermissions.GetValueOrDefault(page, ([], [], []));
                return new PagePermissionsResponseDto
                {
                    Page = page,
                    NecessaryPermissions = permissions.Necessary.ToList(),
                    RecommendedPermissions = permissions.Recommended.ToList(),
                    RequiresDefaultRole = permissions.RequiresDefaultRole.ToList()
                };
            })
            .ToList();
    }
}