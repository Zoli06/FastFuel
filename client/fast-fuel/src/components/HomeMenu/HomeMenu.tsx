import {
  Group,
  Paper as MantinePaper,
  SimpleGrid,
  Stack,
  Text,
  ThemeIcon,
  UnstyledButton,
} from '@mantine/core';
import { Paper } from '../common/Paper/Paper.tsx';
import { Link } from 'react-router-dom';
import { useSuspensePermissions } from '../../hooks/useSuspensePermissions.ts';
import {
  IconAlertCircle,
  IconBook2,
  IconBuildingStore,
  IconChevronRight,
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

export const HomeMenu = () => {
  const can = useSuspensePermissions();
  type Permissions = typeof can;
  const menuItems = [
    {
      text: 'Allergies',
      to: '/manage/allergy',
      visibleWhen: (permissions: Permissions) => permissions.Allergy.Read,
      icon: IconAlertCircle,
      color: 'orange',
    },
    {
      text: 'Admins',
      to: '/manage/admin',
      visibleWhen: (permissions: Permissions) => permissions.Admin.Read,
      icon: IconShield,
      color: 'red',
    },
    {
      text: 'Customers',
      to: 'manage/customer',
      visibleWhen: (permissions: Permissions) => permissions.Customer.Read,
      icon: IconUsers,
      color: 'blue',
    },
    {
      text: 'Employees',
      to: '/manage/employee',
      visibleWhen: (permissions: Permissions) => permissions.Employee.Read,
      icon: IconUsers,
      color: 'blue',
    },
    {
      text: 'Foods',
      to: '/manage/food',
      visibleWhen: (permissions: Permissions) => permissions.Food.Read,
      icon: IconToolsKitchen2,
      color: 'green',
    },
    {
      text: 'Ingredients',
      to: '/manage/ingredient',
      visibleWhen: (permissions: Permissions) => permissions.Ingredient.Read,
      icon: IconLeaf,
      color: 'lime',
    },
    {
      text: 'Menus',
      to: '/manage/menu',
      visibleWhen: (permissions: Permissions) => permissions.Menu.Read,
      icon: IconBook2,
      color: 'violet',
    },
    {
      text: 'Machines',
      to: '/manage/machine',
      visibleWhen: (permissions: Permissions) => permissions.Machine.Read,
      icon: IconDeviceDesktop,
      color: 'gray',
    },
    {
      text: 'Orders',
      to: '/manage/order',
      visibleWhen: (permissions: Permissions) => permissions.Order.Read,
      icon: IconClipboardList,
      color: 'cyan',
    },
    {
      text: 'Create Order',
      to: '/employee/order',
      visibleWhen: (permissions: Permissions) => permissions.Order.Create,
      icon: IconShoppingCart,
      color: 'yellow',
    },
    {
      text: 'Roles',
      to: '/manage/role',
      visibleWhen: (permissions: Permissions) => permissions.Role.Read,
      icon: IconShield,
      color: 'red',
    },
    {
      text: 'Restaurants',
      to: '/manage/restaurant',
      visibleWhen: (permissions: Permissions) => permissions.Restaurant.Read,
      icon: IconBuildingStore,
      color: 'pink',
    },
    {
      text: 'Shifts',
      to: '/manage/shift',
      visibleWhen: (permissions: Permissions) => permissions.Shift.Read,
      icon: IconClock,
      color: 'teal',
    },
    {
      text: 'Station Categories',
      to: '/manage/station-category',
      visibleWhen: (permissions: Permissions) => permissions.StationCategory.Read,
      icon: IconLayoutGrid,
      color: 'grape',
    },
    {
      text: 'Stations',
      to: '/manage/station',
      visibleWhen: (permissions: Permissions) => permissions.Station.Read,
      icon: IconDeviceDesktop,
      color: 'indigo',
    },
  ];

  const visibleItems = menuItems.filter((item) => item.visibleWhen(can));

  const MenuCard = ({ text, to, icon: Icon, color }: (typeof menuItems)[number]) => (
    <UnstyledButton component={Link} to={to} w="100%">
      <MantinePaper withBorder p="md" style={{ transition: 'box-shadow 0.15s' }}>
        <Group justify="space-between" wrap="nowrap">
          <Group gap="sm" wrap="nowrap">
            <ThemeIcon variant="light" color={color} size="lg" radius="md">
              <Icon size={18} />
            </ThemeIcon>
            <Text fw={500} size="sm">
              {text}
            </Text>
          </Group>
          <IconChevronRight
            size={14}
            color="var(--mantine-color-dimmed)"
            style={{ flexShrink: 0 }}
          />
        </Group>
      </MantinePaper>
    </UnstyledButton>
  );

  if (visibleItems.length === 0) {
    return (
      <Paper>
        <Stack align="center" gap="xs" py="xl">
          <IconShield size={32} color="var(--mantine-color-dimmed)" />
          <Text c="dimmed" size="sm">
            No accessible modules
          </Text>
        </Stack>
      </Paper>
    );
  }

  return (
    <Paper>
      <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="sm">
        {visibleItems.map((item) => (
          <MenuCard key={item.to} {...item} />
        ))}
      </SimpleGrid>
    </Paper>
  );
};
