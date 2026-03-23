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
import { myPermissionsQueryOptions } from '../../lib/api-client.ts';
import { useSuspenseQuery } from '@tanstack/react-query';
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

const MENU_ITEMS = [
  {
    text: 'Allergies',
    to: '/manage/allergy',
    permission: 'Permission:Allergy:Read',
    icon: IconAlertCircle,
    color: 'orange',
  },
  {
    text: 'Admins',
    to: '/manage/admin',
    permission: 'Permission:Admin:Read',
    icon: IconShield,
    color: 'red',
  },
  {
    text: 'Employees',
    to: '/manage/employee',
    permission: 'Permission:Employee:Read',
    icon: IconUsers,
    color: 'blue',
  },
  {
    text: 'Foods',
    to: '/manage/food',
    permission: 'Permission:Food:Read',
    icon: IconToolsKitchen2,
    color: 'green',
  },
  {
    text: 'Ingredients',
    to: '/manage/ingredient',
    permission: 'Permission:Ingredient:Read',
    icon: IconLeaf,
    color: 'lime',
  },
  {
    text: 'Machines',
    to: '/manage/machine',
    permission: 'Permission:Machine:Read',
    icon: IconDeviceDesktop,
    color: 'gray',
  },
  {
    text: 'Menus',
    to: '/manage/menu',
    permission: 'Permission:Menu:Read',
    icon: IconBook2,
    color: 'violet',
  },
  {
    text: 'Orders',
    to: '/manage/order',
    permission: 'Permission:Order:Read',
    icon: IconClipboardList,
    color: 'cyan',
  },
  {
    text: 'Create Order',
    to: '/employee/order',
    permission: 'Permission:Order:CreateAtWorkplace',
    icon: IconShoppingCart,
    color: 'yellow',
  },
  {
    text: 'Roles',
    to: '/manage/role',
    permission: 'Permission:Role:Read',
    icon: IconShield,
    color: 'red',
  },
  {
    text: 'Restaurants',
    to: '/manage/restaurant',
    permission: 'Permission:Restaurant:Read',
    icon: IconBuildingStore,
    color: 'pink',
  },
  {
    text: 'Shifts',
    to: '/manage/shift',
    permission: 'Permission:Shift:Read',
    icon: IconClock,
    color: 'teal',
  },
  {
    text: 'Station Categories',
    to: '/manage/station-category',
    permission: 'Permission:StationCategory:Read',
    icon: IconLayoutGrid,
    color: 'grape',
  },
  {
    text: 'Stations',
    to: '/manage/station',
    permission: 'Permission:Station:Read',
    icon: IconDeviceDesktop,
    color: 'indigo',
  },
];

const MenuCard = ({ text, to, icon: Icon, color }: (typeof MENU_ITEMS)[number]) => (
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
        <IconChevronRight size={14} color="var(--mantine-color-dimmed)" style={{ flexShrink: 0 }} />
      </Group>
    </MantinePaper>
  </UnstyledButton>
);

export const HomeMenu = () => {
  const { data: permissions } = useSuspenseQuery(myPermissionsQueryOptions());
  const visibleItems = MENU_ITEMS.filter((item) => permissions?.includes(item.permission as never));

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
