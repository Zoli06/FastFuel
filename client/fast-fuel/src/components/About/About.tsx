import { Divider, List, Stack, Text, ThemeIcon, Title } from '@mantine/core';
import { IconBolt } from '@tabler/icons-react';
import { Paper } from '../common/Paper/Paper.tsx';

const highlights = [
  {
    title: 'Fast ordering',
    description:
      'A straightforward interface for browsing menus, placing orders, and tracking status.',
  },
  {
    title: 'Restaurant operations',
    description: 'Tools for menus, stations, machines, and management on one platform.',
  },
  {
    title: 'Team support',
    description:
      'Dedicated views for staff, admins, and customers based on their roles and permissions.',
  },
];

export const About = () => {
  return (
    <Paper>
      <Stack gap="xl" pb={80}>
        <Stack gap="xs">
          <Title order={2}>About Fast Fuel</Title>
          <Text>
            Fast Fuel is a restaurant management and ordering service built to keep ordering,
            kitchen flow, and staff operations neatly grouped.
          </Text>
        </Stack>

        <Divider />

        <Stack gap="xs">
          <Title order={3}>What we do</Title>
          <Text>
            The system brings customer ordering and internal restaurant workflows under the same
            umbrela so teams can move from order intake to preparation and status tracking with
            smoothness.
          </Text>
        </Stack>

        <Stack gap="xs">
          <Title order={3}>Core areas</Title>
          <List
            spacing="sm"
            icon={
              <ThemeIcon color="red" variant="light" size={24} radius="xl">
                <IconBolt size={14} />
              </ThemeIcon>
            }
          >
            {highlights.map(({ title, description }) => (
              <List.Item key={title}>
                <Text fw={600} component="span">
                  {title}:{' '}
                </Text>
                <Text component="span">{description}</Text>
              </List.Item>
            ))}
          </List>
        </Stack>

        <Stack gap="xs">
          <Title order={3}>Who it is for</Title>
          <Text>
            Fast Fuel is designed for customers placing orders and for restaurant team(s) managing
            menus, staff, stations, and service operations.
          </Text>
        </Stack>
      </Stack>
    </Paper>
  );
};
