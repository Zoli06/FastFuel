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
import {
  type Group as PageGroup,
  type PageDefinition,
  getHomeMenuDefinitions,
} from '../../lib/page-permissions.ts';
import { myRolePagesQueryOptions } from '../../lib/api-client.ts';

export const HomeMenu = () => {
  const { data: pages } = useSuspenseQuery(myRolePagesQueryOptions());
  const groupedDefinitions = getHomeMenuDefinitions(pages);

  const MenuCard = ({
    text,
    to,
    icon: Icon,
    color,
  }: {
    text: string;
    to: string;
    icon: PageDefinition['icon'];
    color: string;
  }) => (
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

  if (Object.keys(groupedDefinitions).length === 0) {
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
      <Stack gap="md">
        {(Object.entries(groupedDefinitions) as [PageGroup, PageDefinition[]][]).map(
          ([group, defs]) => (
            <Stack key={group} gap="xs">
              <Text size="xs" fw={600} c="dimmed" tt="uppercase">
                {group}
              </Text>
              <SimpleGrid cols={{ base: 1, sm: 2, md: 3 }} spacing="sm">
                {defs.map((def) => (
                  <MenuCard
                    key={def.routePath}
                    text={def.displayName}
                    to={def.routePath}
                    icon={def.icon}
                    color={def.color}
                  />
                ))}
              </SimpleGrid>
            </Stack>
          ),
        )}
      </Stack>
    </Paper>
  );
};
