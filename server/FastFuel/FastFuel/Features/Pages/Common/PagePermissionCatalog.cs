using FastFuel.Features.Roles.Common;

namespace FastFuel.Features.Pages.Common;

public static class PagePermissionCatalog
{
    public static readonly IReadOnlyDictionary<Page, (string[] Necessary, string[] Recommended, DefaultRole[] RequiresDefaultRole)> PagePermissions =
        new Dictionary<Page, (string[] Necessary, string[] Recommended, DefaultRole[] RequiresDefaultRole)>
        {
            [Page.StationTasks] =
            (
                ["Permission:Station:ViewTasks"],
                ["Permission:Station:Read", "Permission:Order:UpdateStatus"],
                []
            ),
            [Page.OrderStatusDisplay] =
            (
                ["Permission:Order:Read"],
                ["Permission:Restaurant:Read"],
                []
            ),
            [Page.OrderCreator] =
            (
                [
                    "Permission:Order:Create",
                    "Permission:Restaurant:Read",
                    "Permission:Food:Read",
                    "Permission:Menu:Read"
                ],
                [],
                []
            ),
            [Page.OrderHistory] =
            (
                [
                    "Permission:Order:Read",
                    "Permission:Restaurant:Read",
                    "Permission:Food:Read",
                    "Permission:Menu:Read"],
                [],
                []
            ),
            [Page.AdminManager] =
            (
                ["Permission:Admin:Read"],
                ["Permission:Admin:Create", "Permission:Admin:Update", "Permission:Admin:Delete"],
                []
            ),
            [Page.AllergyManager] =
            (
                ["Permission:Allergy:Read"],
                [
                    "Permission:Allergy:Create",
                    "Permission:Allergy:Update",
                    "Permission:Allergy:Delete",
                    "Permission:Ingredient:Read"
                ],
                []
            ),
            [Page.CustomerManager] =
            (
                ["Permission:Customer:Read"],
                ["Permission:Customer:Update", "Permission:Customer:Delete"],
                []
            ),
            [Page.IngredientManager] =
            (
                ["Permission:Ingredient:Read"],
                [
                    "Permission:Ingredient:Create",
                    "Permission:Ingredient:Update",
                    "Permission:Ingredient:Delete",
                    "Permission:Allergy:Read",
                    "Permission:StationCategory:Read"
                ],
                []
            ),
            [Page.EmployeeManager] =
            (
                ["Permission:Employee:Read"],
                [
                    "Permission:Employee:Create",
                    "Permission:Employee:Update",
                    "Permission:Employee:Delete",
                    "Permission:StationCategory:Read",
                    "Permission:Restaurant:Read"
                ],
                []
            ),
            [Page.MachineManager] =
            (
                ["Permission:Machine:Read"],
                [
                    "Permission:Machine:Create",
                    "Permission:Machine:Update",
                    "Permission:Machine:Delete",
                    "Permission:Restaurant:Read"
                ],
                []
            ),
            [Page.FoodManager] =
            (
                ["Permission:Food:Read"],
                [
                    "Permission:Food:Create",
                    "Permission:Food:Update",
                    "Permission:Food:Delete",
                    "Permission:Ingredient:Read"
                ],
                []
            ),
            [Page.MenuManager] =
            (
                ["Permission:Menu:Read"],
                [
                    "Permission:Menu:Create",
                    "Permission:Menu:Update",
                    "Permission:Menu:Delete",
                    "Permission:Food:Read"
                ],
                []
            ),
            [Page.OrderManager] =
            (
                ["Permission:Order:Read"],
                [
                    "Permission:Order:Create",
                    "Permission:Order:Update",
                    "Permission:Order:Delete",
                    "Permission:Order:UpdateStatus",
                    "Permission:Menu:Read",
                    "Permission:Food:Read",
                    "Permission:User:Read",
                    "Permission:Restaurant:Read"
                ],
                []
            ),
            [Page.RoleManager] =
            (
                ["Permission:Role:Read"],
                [
                    "Permission:Role:Create",
                    "Permission:Role:Update",
                    "Permission:Role:Delete",
                    "Permission:Permission:Read",
                    "Permission:User:Read"
                ],
                [DefaultRole.Admin]
            ),
            [Page.ShiftManager] =
            (
                ["Permission:Shift:Read"],
                [
                    "Permission:Shift:Create",
                    "Permission:Shift:Update",
                    "Permission:Shift:Delete",
                    "Permission:Employee:Read"
                ],
                []
            ),
            [Page.StationCategoryManager] =
            (
                ["Permission:StationCategory:Read"],
                [
                    "Permission:StationCategory:Create",
                    "Permission:StationCategory:Update",
                    "Permission:StationCategory:Delete",
                    "Permission:Ingredient:Read"
                ],
                []
            ),
            [Page.StationManager] =
            (
                ["Permission:Station:Read"],
                [
                    "Permission:Station:ViewTasks",
                    "Permission:Station:Create",
                    "Permission:Station:Update",
                    "Permission:Station:Delete",
                    "Permission:Restaurant:Read",
                    "Permission:StationCategory:Read"
                ],
                []
            ),
            [Page.RestaurantManager] =
            (
                ["Permission:Restaurant:Read"],
                ["Permission:Restaurant:Create", "Permission:Restaurant:Update", "Permission:Restaurant:Delete"],
                []
            ),
            [Page.Profile] =
            (
                [],
                [],
                []
            ),
        };
}