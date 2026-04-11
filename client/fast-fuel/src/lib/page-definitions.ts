import {
  IconAlertCircle,
  IconBook2,
  IconBuildingStore,
  IconClipboardList,
  IconClock,
  IconDeviceDesktop,
  IconHistory,
  IconLayoutGrid,
  IconLeaf,
  IconShield,
  IconShoppingCart,
  IconToolsKitchen2,
  IconUser,
  IconUsers,
  IconAddressBook,
  IconUsersGroup,
  IconChefHat,
  IconListCheckFilled,
} from '@tabler/icons-react';
import type { components, operations } from '../types/api-schema.generated.ts';

export type Page = components['schemas']['Page'];
export type Permission =
  operations['Permission_GetAll']['responses'][200]['content']['application/json'][number];

export type Group = 'Edibles' | 'Restaurant' | 'User Management' | 'Orders' | 'Me' | 'Display';

type IconComponent = typeof IconShield;

export interface PageDefinition {
  displayName: string;
  color: string;
  icon: IconComponent;
  routePath: string;
  showInHomeMenu: boolean;
  group: Group;
}

export const pageDefinitions = {
  StationTasks: {
    displayName: 'Station Tasks',
    color: 'indigo',
    icon: IconChefHat,
    routePath: '/station-tasks',
    showInHomeMenu: true,
    group: 'Display',
  },
  OrderStatusDisplay: {
    displayName: 'Order Status Display',
    color: 'blue',
    icon: IconListCheckFilled,
    routePath: '/restaurants/order-status-display',
    showInHomeMenu: true,
    group: 'Display',
  },
  OrderCreator: {
    displayName: 'Create Order',
    color: 'green',
    icon: IconShoppingCart,
    routePath: '/order',
    showInHomeMenu: true,
    group: 'Orders',
  },
  OrderHistory: {
    displayName: 'Order History',
    color: 'cyan',
    icon: IconHistory,
    routePath: '/order-history',
    showInHomeMenu: true,
    group: 'Orders',
  },
  AdminManager: {
    displayName: 'Admins',
    color: 'red',
    icon: IconShield,
    routePath: '/manage/admin',
    showInHomeMenu: true,
    group: 'User Management',
  },
  AllergyManager: {
    displayName: 'Allergies',
    color: 'orange',
    icon: IconAlertCircle,
    routePath: '/manage/allergy',
    showInHomeMenu: true,
    group: 'Edibles',
  },
  CustomerManager: {
    displayName: 'Customers',
    color: 'green',
    icon: IconUsersGroup,
    routePath: '/manage/customer',
    showInHomeMenu: true,
    group: 'User Management',
  },
  IngredientManager: {
    displayName: 'Ingredients',
    color: 'lime',
    icon: IconLeaf,
    routePath: '/manage/ingredient',
    showInHomeMenu: true,
    group: 'Edibles',
  },
  EmployeeManager: {
    displayName: 'Employees',
    color: 'blue',
    icon: IconUsers,
    routePath: '/manage/employee',
    showInHomeMenu: true,
    group: 'User Management',
  },
  Profile: {
    displayName: 'Profile',
    color: 'blue',
    icon: IconUser,
    routePath: '/profile',
    showInHomeMenu: true,
    group: 'Me',
  },
  MachineManager: {
    displayName: 'Machines',
    color: 'gray',
    icon: IconDeviceDesktop,
    routePath: '/manage/machine',
    showInHomeMenu: true,
    group: 'User Management',
  },
  FoodManager: {
    displayName: 'Foods',
    color: 'green',
    icon: IconToolsKitchen2,
    routePath: '/manage/food',
    showInHomeMenu: true,
    group: 'Edibles',
  },
  MenuManager: {
    displayName: 'Menus',
    color: 'violet',
    icon: IconBook2,
    routePath: '/manage/menu',
    showInHomeMenu: true,
    group: 'Edibles',
  },
  OrderManager: {
    displayName: 'Orders',
    color: 'cyan',
    icon: IconClipboardList,
    routePath: '/manage/order',
    showInHomeMenu: true,
    group: 'Orders',
  },
  RoleManager: {
    displayName: 'Roles',
    color: 'cyan',
    icon: IconAddressBook,
    routePath: '/manage/role',
    showInHomeMenu: true,
    group: 'User Management',
  },
  ShiftManager: {
    displayName: 'Shifts',
    color: 'teal',
    icon: IconClock,
    routePath: '/manage/shift',
    showInHomeMenu: true,
    group: 'Restaurant',
  },
  StationCategoryManager: {
    displayName: 'Station Categories',
    color: 'grape',
    icon: IconLayoutGrid,
    routePath: '/manage/station-category',
    showInHomeMenu: true,
    group: 'Restaurant',
  },
  StationManager: {
    displayName: 'Stations',
    color: 'indigo',
    icon: IconDeviceDesktop,
    routePath: '/manage/station',
    showInHomeMenu: true,
    group: 'Restaurant',
  },
  RestaurantManager: {
    displayName: 'Restaurants',
    color: 'pink',
    icon: IconBuildingStore,
    routePath: '/manage/restaurant',
    showInHomeMenu: true,
    group: 'Restaurant',
  },
  MyShifts: {
    displayName: 'My Shifts',
    color: 'teal',
    icon: IconClock,
    routePath: '/employee/my-shifts',
    showInHomeMenu: true,
    group: 'Me',
  },
} as const satisfies Record<Page, PageDefinition>;

export const getPageDefinitions = (pages: Page[]): Partial<Record<Group, PageDefinition[]>> => {
  const allowed = new Set(pages);
  const result: Partial<Record<Group, PageDefinition[]>> = {};

  for (const page of Object.keys(pageDefinitions) as Page[]) {
    const def = pageDefinitions[page];
    if (!allowed.has(page) || !def.showInHomeMenu) continue;
    (result[def.group] ??= []).push(def);
  }

  return result;
};
