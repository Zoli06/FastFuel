using FastFuel.Features.Roles.Common;

namespace FastFuel.Features.Pages.Common;

public static class PagePermissionCatalog
{
    public static readonly
        IReadOnlyDictionary<Page, PagePermissionCatalogItem>
        PagePermissions =
            new Dictionary<Page, PagePermissionCatalogItem>
            {
                [Page.StationTasks] =
                    new(
                        ["Permission:Station:ViewTasks"],
                        ["Permission:Station:Read", "Permission:Order:UpdateStatus"],
                        [],
                        []
                    ),
                [Page.OrderStatusDisplay] =
                    new(
                        ["Permission:Order:Read"],
                        [],
                        [],
                        []
                    ),
                [Page.OrderCreator] =
                    new(
                        [
                            "Permission:Order:Create",
                            "Permission:Food:Read",
                            "Permission:Menu:Read",
                            "Permission:Restaurant:Read"
                        ],
                        [],
                        [],
                        []
                    ),
                [Page.AdminManager] =
                    new(
                        ["Permission:Admin:Read"],
                        ["Permission:Admin:Create", "Permission:Admin:Update", "Permission:Admin:Delete"],
                        [],
                        []
                    ),
                [Page.AllergyManager] =
                    new(
                        ["Permission:Allergy:Read"],
                        [
                            "Permission:Allergy:Create",
                            "Permission:Allergy:Update",
                            "Permission:Allergy:Delete",
                            "Permission:Ingredient:Read"
                        ],
                        [],
                        []
                    ),
                [Page.CustomerManager] =
                    new(
                        ["Permission:Customer:Read"],
                        ["Permission:Customer:Update", "Permission:Customer:Delete"],
                        [],
                        []
                    ),
                [Page.IngredientManager] =
                    new(
                        ["Permission:Ingredient:Read"],
                        [
                            "Permission:Ingredient:Create",
                            "Permission:Ingredient:Update",
                            "Permission:Ingredient:Delete",
                            "Permission:Allergy:Read",
                            "Permission:StationCategory:Read"
                        ],
                        [],
                        []
                    ),
                [Page.EmployeeManager] =
                    new(
                        ["Permission:Employee:Read"],
                        [
                            "Permission:Employee:Create",
                            "Permission:Employee:Update",
                            "Permission:Employee:Delete",
                            "Permission:StationCategory:Read"
                        ],
                        [],
                        []
                    ),
                [Page.MachineManager] =
                    new(
                        ["Permission:Machine:Read"],
                        [
                            "Permission:Machine:Create",
                            "Permission:Machine:Update",
                            "Permission:Machine:Delete"
                        ],
                        [],
                        []
                    ),
                [Page.FoodManager] =
                    new(
                        ["Permission:Food:Read"],
                        [
                            "Permission:Food:Create",
                            "Permission:Food:Update",
                            "Permission:Food:Delete",
                            "Permission:Ingredient:Read"
                        ],
                        [],
                        []
                    ),
                [Page.MenuManager] =
                    new(
                        ["Permission:Menu:Read"],
                        [
                            "Permission:Menu:Create",
                            "Permission:Menu:Update",
                            "Permission:Menu:Delete",
                            "Permission:Food:Read"
                        ],
                        [],
                        []
                    ),
                [Page.OrderManager] =
                    new(
                        ["Permission:Order:Read"],
                        [
                            "Permission:Order:Create",
                            "Permission:Order:Update",
                            "Permission:Order:Delete",
                            "Permission:Order:UpdateStatus",
                            "Permission:Menu:Read",
                            "Permission:Food:Read",
                            "Permission:User:Read"
                        ],
                        [],
                        []
                    ),
                [Page.RoleManager] =
                    new(
                        ["Permission:Role:Read"],
                        [
                            "Permission:Role:Create",
                            "Permission:Role:Update",
                            "Permission:Role:Delete",
                            "Permission:Permission:Read",
                            "Permission:User:Read"
                        ],
                        [DefaultRole.Admin],
                        [DefaultRole.Admin]
                    ),
                [Page.ShiftManager] =
                    new(
                        ["Permission:Shift:Read"],
                        [
                            "Permission:Shift:Create",
                            "Permission:Shift:Update",
                            "Permission:Shift:Delete",
                            "Permission:Employee:Read"
                        ],
                        [],
                        []
                    ),
                [Page.StationCategoryManager] =
                    new(
                        ["Permission:StationCategory:Read"],
                        [
                            "Permission:StationCategory:Create",
                            "Permission:StationCategory:Update",
                            "Permission:StationCategory:Delete",
                            "Permission:Ingredient:Read"
                        ],
                        [],
                        []
                    ),
                [Page.StationManager] =
                    new(
                        ["Permission:Station:Read"],
                        [
                            "Permission:Station:Create",
                            "Permission:Station:Update",
                            "Permission:Station:Delete",
                            "Permission:StationCategory:Read"
                        ],
                        [],
                        []
                    ),
                [Page.RestaurantManager] =
                    new(
                        [],
                        [
                            "Permission:Restaurant:Create", "Permission:Restaurant:Update",
                            "Permission:Restaurant:Delete"
                        ],
                        [],
                        []
                    ),
                [Page.MyShifts] = new([], [], [DefaultRole.Employee], []),
                [Page.Profile] =
                    new(
                        [],
                        [
                            "Permission:Customer:UpdateSelf",
                            "Permission:StationCategory:Read"
                        ],
                        [],
                        []
                    ),
                [Page.OrderHistory] =
                    new(
                        [
                            "Permission:Food:Read", "Permission:Menu:Read"
                        ],
                        [],
                        [],
                        []
                    )
            };

    public readonly record struct PagePermissionCatalogItem(
        string[] Necessary,
        string[] Recommended,
        DefaultRole[] RequiresDefaultRole,
        DefaultRole[] RequiredForDefaultRole);
}