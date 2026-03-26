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
import { useSuspenseQuery } from '@tanstack/react-query';
import { IconChevronRight, IconShield } from '@tabler/icons-react';
import { myPermissionsQueryOptions } from '../../lib/api-client.ts';
import { buildPermissionMap } from '../../lib/buildPermissionMap.ts';
import { pageDefinitions } from '../../lib/page-permissions.ts';

export const HomeMenu = () => {
  const { data: permissions } = useSuspenseQuery(myPermissionsQueryOptions());
  const permissionMap = buildPermissionMap(permissions) as Record<string, Record<string, boolean>>;
  const homePages = Object.values(pageDefinitions).filter((definition) => {
    if (!definition.showInHomeMenu) {
      return false;
    }

    return definition.necessaryPermissions.every((permission) => {
      const [, resource, action] = permission.split(':');
      return permissionMap[resource][action];
    });
  });

  const menuItems = homePages.map((definition) => ({
    text: definition.displayName,
    to: definition.routePath,
    icon: definition.icon,
    color: definition.color,
  }));

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

  if (menuItems.length === 0) {
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
        {menuItems.map((item) => (
          <MenuCard key={item.to} {...item} />
        ))}
      </SimpleGrid>
    </Paper>
  );
};
