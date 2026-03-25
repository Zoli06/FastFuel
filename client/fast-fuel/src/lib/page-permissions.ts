import {
  IconAlertCircle,
  IconBook2,
  IconBuildingStore,
  IconClipboardList,
  IconClock,
  IconDeviceDesktop,
  IconLayoutGrid,
  IconLeaf,
  IconShield,
  IconShoppingCart,
  IconToolsKitchen2,
  IconUsers,
} from '@tabler/icons-react';
import type { components, operations } from '../types/api';

export type Page = components['schemas']['Page'];
export type Permission =
  operations['Permission_GetAll']['responses'][200]['content']['application/json'][number];

type IconComponent = typeof IconShield;

export interface PageDefinition {
  displayName: string;
  color: string;
  icon: IconComponent;
  routePath: string;
  showInHomeMenu: boolean;
  necessaryPermissions: readonly Permission[];
  recommendedPermissions: readonly Permission[];
}

export const pageDefinitions = {
  StationTasks: {
    displayName: 'Station Tasks',
    color: 'indigo',
    icon: IconClipboardList,
    routePath: '/stations/:id/tasks',
    showInHomeMenu: false,
    necessaryPermissions: ['Permission:Station:ViewTasks'],
    recommendedPermissions: ['Permission:Station:Read', 'Permission:Order:UpdateStatus'],
  },
  OrderStatusDisplay: {
    displayName: 'Order Status Display',
    color: 'cyan',
    icon: IconClipboardList,
    routePath: '/restaurants/:id/status-display',
    showInHomeMenu: false,
    necessaryPermissions: ['Permission:Order:Read'],
    recommendedPermissions: ['Permission:Order:UpdateStatus', 'Permission:Restaurant:Read'],
  },
  EmployeeOrder: {
    displayName: 'Create Order',
    color: 'yellow',
    icon: IconShoppingCart,
    routePath: '/employee/order',
    showInHomeMenu: true,
    necessaryPermissions: [
      'Permission:Order:Create',
      'Permission:Order:CreateAtWorkplace',
      'Permission:Order:Read',
      'Permission:Food:Read',
      'Permission:Menu:Read',
    ],
    recommendedPermissions: [],
  },
  AdminManager: {
    displayName: 'Admins',
    color: 'red',
    icon: IconShield,
    routePath: '/manage/admin',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Admin:Read'],
    recommendedPermissions: [
      'Permission:Admin:Create',
      'Permission:Admin:Update',
      'Permission:Admin:Delete',
    ],
  },
  AllergyManager: {
    displayName: 'Allergies',
    color: 'orange',
    icon: IconAlertCircle,
    routePath: '/manage/allergy',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Allergy:Read'],
    recommendedPermissions: [
      'Permission:Allergy:Create',
      'Permission:Allergy:Update',
      'Permission:Allergy:Delete',
      'Permission:Ingredient:Read',
    ],
  },
  CustomerManager: {
    displayName: 'Customers',
    color: 'blue',
    icon: IconUsers,
    routePath: '/manage/customer',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Customer:Read'],
    recommendedPermissions: ['Permission:Customer:Update', 'Permission:Customer:Delete'],
  },
  IngredientManager: {
    displayName: 'Ingredients',
    color: 'lime',
    icon: IconLeaf,
    routePath: '/manage/ingredient',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Ingredient:Read'],
    recommendedPermissions: [
      'Permission:Ingredient:Create',
      'Permission:Ingredient:Update',
      'Permission:Ingredient:Delete',
      'Permission:Allergy:Read',
      'Permission:StationCategory:Read',
    ],
  },
  EmployeeManager: {
    displayName: 'Employees',
    color: 'blue',
    icon: IconUsers,
    routePath: '/manage/employee',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Employee:Read'],
    recommendedPermissions: [
      'Permission:Employee:Create',
      'Permission:Employee:Update',
      'Permission:Employee:Delete',
      'Permission:StationCategory:Read',
      'Permission:Restaurant:Read',
    ],
  },
  MachineManager: {
    displayName: 'Machines',
    color: 'gray',
    icon: IconDeviceDesktop,
    routePath: '/manage/machine',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Machine:Read'],
    recommendedPermissions: [
      'Permission:Machine:Create',
      'Permission:Machine:Update',
      'Permission:Machine:Delete',
      'Permission:Restaurant:Read',
    ],
  },
  FoodManager: {
    displayName: 'Foods',
    color: 'green',
    icon: IconToolsKitchen2,
    routePath: '/manage/food',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Food:Read'],
    recommendedPermissions: [
      'Permission:Food:Create',
      'Permission:Food:Update',
      'Permission:Food:Delete',
      'Permission:Ingredient:Read',
    ],
  },
  MenuManager: {
    displayName: 'Menus',
    color: 'violet',
    icon: IconBook2,
    routePath: '/manage/menu',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Menu:Read'],
    recommendedPermissions: [
      'Permission:Menu:Create',
      'Permission:Menu:Update',
      'Permission:Menu:Delete',
      'Permission:Food:Read',
    ],
  },
  OrderManager: {
    displayName: 'Orders',
    color: 'cyan',
    icon: IconClipboardList,
    routePath: '/manage/order',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Order:Read'],
    recommendedPermissions: [
      'Permission:Order:Create',
      'Permission:Order:Update',
      'Permission:Order:Delete',
      'Permission:Order:UpdateStatus',
      'Permission:Menu:Read',
      'Permission:Food:Read',
      'Permission:User:Read',
      'Permission:Restaurant:Read',
    ],
  },
  RoleManager: {
    displayName: 'Roles',
    color: 'red',
    icon: IconShield,
    routePath: '/manage/role',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Role:Read'],
    recommendedPermissions: [
      'Permission:Role:Create',
      'Permission:Role:Update',
      'Permission:Role:Delete',
      'Permission:Permission:Read',
      'Permission:User:Read',
    ],
  },
  ShiftManager: {
    displayName: 'Shifts',
    color: 'teal',
    icon: IconClock,
    routePath: '/manage/shift',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Shift:Read'],
    recommendedPermissions: [
      'Permission:Shift:Create',
      'Permission:Shift:Update',
      'Permission:Shift:Delete',
      'Permission:Employee:Read',
    ],
  },
  StationCategoryManager: {
    displayName: 'Station Categories',
    color: 'grape',
    icon: IconLayoutGrid,
    routePath: '/manage/station-category',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:StationCategory:Read'],
    recommendedPermissions: [
      'Permission:StationCategory:Create',
      'Permission:StationCategory:Update',
      'Permission:StationCategory:Delete',
      'Permission:Ingredient:Read',
    ],
  },
  StationManager: {
    displayName: 'Stations',
    color: 'indigo',
    icon: IconDeviceDesktop,
    routePath: '/manage/station',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Station:Read'],
    recommendedPermissions: [
      'Permission:Station:ViewTasks',
      'Permission:Station:Create',
      'Permission:Station:Update',
      'Permission:Station:Delete',
      'Permission:Restaurant:Read',
      'Permission:StationCategory:Read',
    ],
  },
  RestaurantManager: {
    displayName: 'Restaurants',
    color: 'pink',
    icon: IconBuildingStore,
    routePath: '/manage/restaurant',
    showInHomeMenu: true,
    necessaryPermissions: ['Permission:Restaurant:Read'],
    recommendedPermissions: [
      'Permission:Restaurant:Create',
      'Permission:Restaurant:Update',
      'Permission:Restaurant:Delete',
    ],
  },
} as const satisfies Record<Page, PageDefinition>;

export const getHomeMenuDefinitions = (pages: Page[]): PageDefinition[] =>
  pages.map((page) => pageDefinitions[page]).filter((definition) => definition.showInHomeMenu);
